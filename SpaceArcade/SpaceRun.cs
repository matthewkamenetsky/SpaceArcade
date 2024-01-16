/* Author: Matthew Kamenetsky
 * File name: SpaceRun.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 26, 2023
 * Modified date: June 10, 2023
 * Description: The SpaceRun class is responsible for updating the gameplay of the space run minigame. It manages all of the space objects and their collision with the player. It updates and draws the player and the space objects.  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;
using Helper;

namespace SpaceArcade
{
    class SpaceRun : Minigame
    {
        //Store the scrolling screen direction
        private const int DIR = -1;

        //Store the bottom tolerance
        private const int BOT_TOLERANCE = 120;

        //Store the max in one direction time and damage tick time
        private const double MAX_DIR_TIME = 2000;
        private const double TICK_TIME = 1000;

        //Store soundeffect indexes
        public const int SCREAM = 0;
        public const int BOOST = 1;
        public const int DODGE = 2;
        public const int TICK = 3;

        //Store the indexes of each space object
        public const int ASTEROID = 0;
        public const int FIRE = 1;
        public const int FAKE_COIN = 2;
        public const int COIN = 3;

        //Store the number of space objects spawning
        private const int TO_SPAWN = 58;

        //Store the background rectangles and positions
        private Rectangle bgRec2;
        private Vector2 bgPos1;
        private Vector2 bgPos2;

        //Store the list of space objects and the object queue
        private List<SpaceObject> spaceObjects = new List<SpaceObject>();
        private SpaceObjectQueue objectQueue = new SpaceObjectQueue(TO_SPAWN);

        //Store the space object images and the blank texture used for their hitboxes
        private Texture2D[] spaceObjImgs;
        private Texture2D blankTexture;

        //Store data related to the space objects
        private int[] spawnChances = new int[] { 30, 30, 30, 10 };
        private float[] objSpeeds = new float[] { 8f, 6f, 7.5f, 10f };
        private int dodgePts = 10;
        private int hitPts = 5;
        private int coinBoost = 5;
        private int dmgMultiplier = 1;

        //Store data related to the background scrolling
        private float scrollSpeedMax = 200f;
        private float scrollSpeed;

        //Store the spawn time and timer
        private double spawnTime = 1000;
        private Timer spawnTimer;

        //Store the boost time and timer
        private double boostTime = 2250;
        private Timer boostTimer;

        //Store the damage increase time and timer
        private double nerfTime = 1750;
        private Timer nerfTimer;

        //Store the tick timer
        private Timer tickTimer;
        
        //Pre: fonts are the game fonts, bg is the game's background image, spaceObjImgs are the space object images, blankTexture is a blank image, gameSnds are the game's sounds, screenWidth and screenHeight are the screen dimensions
        //Post: none
        //Desc: sets up the space run game with necessary values
        public SpaceRun(SpriteFont[] fonts, Texture2D bg, Texture2D[] spaceObjImgs, Texture2D blankTexture, SoundEffect[] gameSnds, int screenWidth, int screenHeight) : base(fonts, bg, gameSnds, screenWidth, screenHeight) 
        {
            //Setup the 2nd background rectangle and the background positions
            bgRec2 = new Rectangle(screenWidth, 0, screenWidth, screenHeight);
            bgPos1 = new Vector2(bgRec.X, bgRec.Y);
            bgPos2 = new Vector2(bgRec2.X, bgRec2.Y);

            //Setup the space object images and their hitbox image
            this.spaceObjImgs = spaceObjImgs;
            this.blankTexture = blankTexture;

            //Setup the timers
            spawnTimer = new Timer(spawnTime, true);
            boostTimer = new Timer(boostTime, false);
            nerfTimer = new Timer(nerfTime, false);
            tickTimer = new Timer(Timer.INFINITE_TIMER, false);

            //Setup the game
            Setup();
        }

        //Pre: none
        //Post: returns the integer corresponding to the game
        //Desc: returns the integer corresponding to the game, to be used by the driver
        public override int GetType()
        {
            //Return that this game is the runner game
            return Game1.RUNNER;
        }

        //Pre: none
        //Post: none
        //Desc: sets up the space run game by queuing up random space objects
        private void Setup() 
        {
            //Store the random integer representing the next space object
            int nextSpawn;

            //Store the position of the space object's animation
            Vector2 animPos = new Vector2(0, 0);

            //Store the dimensions of the space object
            int imgHeight;
            int imgWidth;

            //Loop through the number of space objects to spawn
            for (int i = 0; i < TO_SPAWN; i++) 
            {
                //Store the random next spawn
                nextSpawn = Game1.rng.Next(1, 101);

                //Compare the next spawn to the spawn chances of each object in order to create it
                if (nextSpawn <= spawnChances[ASTEROID])
                {
                    //Calculate the image's height, set the object position, and enqueue the asteroid space object
                    imgHeight = new RunnerAsteroid(spaceObjImgs[ASTEROID], blankTexture, animPos, objSpeeds[ASTEROID]).GetHeight();
                    animPos.X = screenWidth;
                    animPos.Y = Game1.rng.Next(screenHeight / 2 - imgHeight, screenHeight - BOT_TOLERANCE - imgHeight + 1);
                    objectQueue.Enqueue(new RunnerAsteroid(spaceObjImgs[ASTEROID], blankTexture, animPos, objSpeeds[ASTEROID]));
                }
                else if (nextSpawn <= (spawnChances[ASTEROID] + spawnChances[FIRE]))
                {
                    //Calculate the image's width, set the object position, and enqueue the fireball space object
                    imgWidth = new Fireball(spaceObjImgs[FIRE], blankTexture, animPos, objSpeeds[FIRE]).GetWidth();
                    animPos.X = Game1.rng.Next((screenWidth + imgWidth) / 2, (int)(screenWidth - imgWidth * 1.2 + 1));
                    animPos.Y = -spaceObjImgs[FIRE].Height;
                    objectQueue.Enqueue(new Fireball(spaceObjImgs[FIRE], blankTexture, animPos, objSpeeds[FIRE]));
                }
                else if (nextSpawn <= (spawnChances[ASTEROID] + spawnChances[FIRE] + spawnChances[FAKE_COIN]))
                {
                    //Calculate the image's height, set the object position, and enqueue the fake coin space object
                    imgHeight = new FakeCoin(spaceObjImgs[FAKE_COIN], blankTexture, animPos, objSpeeds[FAKE_COIN]).GetHeight();
                    animPos.X = screenWidth;
                    animPos.Y = Game1.rng.Next(0, screenHeight / 2 - imgHeight + 1);
                    objectQueue.Enqueue(new FakeCoin(spaceObjImgs[FAKE_COIN], blankTexture, animPos, objSpeeds[FAKE_COIN]));
                }
                else 
                {
                    //Calculate the image's height, set the object position, and enqueue the coin space object
                    imgHeight = new Coin(spaceObjImgs[COIN], blankTexture, animPos, objSpeeds[COIN]).GetHeight();
                    animPos.X = screenWidth;
                    animPos.Y = Game1.rng.Next(0, screenHeight - BOT_TOLERANCE - imgHeight + 1);
                    objectQueue.Enqueue(new Coin(spaceObjImgs[COIN], blankTexture, animPos, objSpeeds[COIN]));
                }
            }
        }

        //Pre: gameTime is the number of time passed in the game, player is the player, kb and prevKb are the keyboard input states, mouse and prevMouse are the mouse input states
        //Post: none
        //Desc: updates the space run game by updating timers, spawning space objects, and detecting collision
        public override void UpdateGame(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            //Updates the base elements of the game
            base.UpdateGame(gameTime, player, kb, prevKb, mouse, prevMouse);

            //Updates the timers
            spawnTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            boostTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            nerfTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            tickTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Scrolls the screen
            ScrollScreen((float)gameTime.ElapsedGameTime.TotalSeconds);

            //Updates the player
            player.Update(gameTime, Game1.RUNNER, screenWidth, screenHeight, kb, prevKb, mouse, prevMouse);

            //Check if the time the player goes in one direction is greater than the max one direction time
            if (player.GetDirTime(Player.FLAME) >= MAX_DIR_TIME || player.GetDirTime(Player.FIZZLE) >= MAX_DIR_TIME)
            {
                //Check if the tick timer is not active
                if (!tickTimer.IsActive())
                {
                    //Reset the tick timer
                    tickTimer.ResetTimer(true);
                }
            }
            else 
            {
                //Reset the tick timer to false
                tickTimer.ResetTimer(false);
            }

            //Check if the tick timer has passed the tick interval
            if (tickTimer.GetTimePassed() >= TICK_TIME)
            {
                //Play the tick sound
                gameSnds[TICK].CreateInstance().Play();

                //Decrease the points by the regular hitpoints and reset the tick timer to false
                points = Math.Max(0, points - hitPts);
                tickTimer.ResetTimer(false);
            }

            //Checks if the spawn timer is not active
            if (!spawnTimer.IsActive())
            {
                //Checks if the objectqueue is greater than 0
                if (objectQueue.Size() > 0)
                {
                    //Add the dequeued space object into the list of on screen space objects
                    spaceObjects.Add(objectQueue.Dequeue());
                }

                //Reset the spawn timer
                spawnTimer.ResetTimer(true);
            }

            //Check if the nerf timer is not active to reset the damage multiplier
            if (!nerfTimer.IsActive())
            {
                //Reset the damage multiplier
                dmgMultiplier = 1;
            }

            //Loop through the space objects in order to update them
            for (int i = 0; i < spaceObjects.Count; i++)
            {
                //Update the space objects
                spaceObjects[i].Update(gameTime);
            }

            //Check collision
            HasCollided(player);
        }

        //Pre: player is the player
        //Post: none
        //Desc: checks collision between the player and space objects, and the space objects and the edge of the screen
        public override void HasCollided(Player player)
        {
            //Loop through all the space objects to check collision
            for (int i = 0; i < spaceObjects.Count; i++) 
            {
                //Check the space object's collision
                if (spaceObjects[i].GetHitBox().Intersects(player.GetRec()))
                {
                    //Check if a coin was hit
                    if (spaceObjects[i] is Coin)
                    {
                        //Check if the boost timer is not active
                        if (!boostTimer.IsActive())
                        {
                            //Play the boost sound
                            gameSnds[BOOST].CreateInstance().Play();

                            //Reset the boost timer
                            boostTimer.ResetTimer(true);
                        }
                    }
                    else
                    {
                        //Play the scream sound effect
                        gameSnds[SCREAM].CreateInstance().Play();

                        //Check if the space object is a fake coin to decrease the points even more
                        if (spaceObjects[i] is FakeCoin)
                        {
                            //Decrease the points by a larger number
                            points = Math.Max(0, points - dodgePts * dmgMultiplier);
                        }
                        else 
                        {
                            //Decrease the points by the regular hitpoints
                            points = Math.Max(0, points - hitPts * dmgMultiplier);
                        }

                        //Check if the nerf timer is not yet active
                        if (!nerfTimer.IsActive())
                        {
                            //Increase the damage multiplier and start the nerf timer
                            dmgMultiplier = 2;
                            nerfTimer.ResetTimer(true);
                        }
                    }

                    //Remove the object
                    spaceObjects.RemoveAt(i);
                }
                else if (spaceObjects[i].GetRec().Right <= 0) 
                {
                    //Play the dodge sound effect
                    gameSnds[DODGE].CreateInstance().Play();

                    //Check if the object is not a coin
                    if (!(spaceObjects[i] is Coin))
                    {
                        //Check if the boost timer is active
                        if (boostTimer.IsActive())
                        {
                            //Increase the points by the dodge points and the coin boost
                            points += dodgePts + coinBoost;
                        }
                        else 
                        {
                            //Increase the points by the dodge points
                            points += dodgePts;
                        }
                    }
                    else 
                    {
                        //Decrease the points by the dodge points
                        points = Math.Max(0, points - dodgePts);
                    }

                    //Remove the object
                    spaceObjects.RemoveAt(i);
                }
            }
        }

        //Pre: deltaTime is the time passed in the game
        //Post: none
        //Desc: scrolls the screen by moving the two background rectangles
        private void ScrollScreen(float deltaTime) 
        {
            //Calculates the scrollspeed
            scrollSpeed = DIR * scrollSpeedMax * deltaTime;

            //Adds the scrollspeed to both background positions
            bgPos1.X += scrollSpeed;
            bgPos2.X += scrollSpeed;

            //Checks if the x position of the first image has went offscreen
            if (bgPos1.X <= -screenWidth)
            {
                //Add two screenwidths to the x position
                bgPos1.X += screenWidth * 2;
            }

            //Checks if the x position of the second image has went offscreen
            if (bgPos2.X <= -screenWidth)
            {
                //Add two screenwidths to the x position
                bgPos2.X += screenWidth * 2;
            }

            //Update the rectangle x positions
            bgRec.X = (int)bgPos1.X;
            bgRec2.X = (int)bgPos2.X;
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images, player is the player
        //Post: none
        //Desc: draws all elements of the space run game, including text, space objects, and the player
        public override void Draw(SpriteBatch spriteBatch, Player player)
        {
            //Draw the background images
            spriteBatch.Draw(bgImg, bgRec, Color.White);
            spriteBatch.Draw(bgImg, bgRec2, Color.White);

            //Draw the player
            player.Draw(spriteBatch, GetType());

            //Loop through each space object to draw it
            foreach (SpaceObject obj in spaceObjects)
            {
                //Draw the space object
                obj.Draw(spriteBatch);
            }

            //Draw the base parts of the game
            base.Draw(spriteBatch, player);
        }
    }
}
