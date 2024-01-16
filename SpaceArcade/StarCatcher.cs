/* Author: Matthew Kamenetsky
 * File name: StarCatcher.cs
 * Project name: PASS3_ICS4U
 * Creation date: June 04, 2023
 * Modified date: June 08, 2023
 * Description: The StarCatcher class is responsible for updating the gameplay of the star catcher minigame. It manages all of the stars and asteroids, and their collision with the player and each other, as well as with the screen dimensions. It updates and draws the player and these objects.  
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
    class StarCatcher : Minigame
    {
        //Store the sound effect indexes
        public const int BTN = 0;
        public const int CATCH = 1;
        public const int BOUNCE = 2;

        //Store the indexes of the star sizes
        public const int BIG = 0;
        public const int MED = 1;
        public const int SML = 2;

        //Store the heights of the stars
        private const int BIG_SIZE = 55;
        private const int MED_SIZE = 47;
        private const int SML_SIZE = 36;

        //Store direction indexes
        public const int LEFT = -1;
        public const int RIGHT = 1;
        public const int DOWN = -1;
        public const int UP = 1;

        //Store position indexes
        public const int X = 0;
        public const int Y = 1;

        //Store the asteroid speed
        private const float ASTEROID_SPEED = 4f;

        //Store the selector
        private Selector selector;

        //Store whether or not the player's speed has already changed from the selector
        private bool hasChangedSpeed = false;

        //Store the stars and their information
        private Star[] stars = new Star[3];
        private Texture2D[] starImgs;
        private Vector2[] startLocs = new Vector2[3];
        private int[] starPoints = new int[] { 30, 40, 50 };

        //Store the asteroid image and its hitbox image
        private Texture2D asteroidImg;
        private Texture2D blankTexture;

        //Store the list of asteroids
        private List<CatcherAsteroid> asteroids = new List<CatcherAsteroid>();

        //Store the asteroid timer and time
        private Timer asteroidTimer;
        private double asteroidTime= 1000;

        //Store the star collision wait timer and time
        private Timer waitTimer;
        private double waitTime = 1500;

        //Pre: fonts are the game fonts, bg is the game's background image, selectorImgs are the selector images, starImgs are the star images, asteroidImg is the asteroid image, gameSnds are the game's sounds, screenWidth and screenHeight are the screen dimensions
        //Post: none
        //Desc: sets up the star catcher game with necessary values
        public StarCatcher(SpriteFont[] fonts, Texture2D bg, Texture2D[] selectorImgs, Texture2D[] starImgs, Texture2D asteroidImg, SoundEffect[] gameSnds, int screenWidth, int screenHeight) : base(fonts, bg, gameSnds, screenWidth, screenHeight)
        {
            //Create the selector
            selector = new Selector(fonts[Game1.BOLD], selectorImgs, screenWidth, screenHeight);

            //Setup the star images
            this.starImgs = starImgs;

            //Setup the asteroid image and its hitbox image
            this.asteroidImg = asteroidImg;
            blankTexture = selectorImgs[Selector.BLANK];

            //Setup the timers
            asteroidTimer = new Timer(asteroidTime, false);
            waitTimer = new Timer(waitTime, false);

            //Create the starting locations of the stars
            startLocs[BIG] = new Vector2(screenWidth / 2 - Game1.TITLE_OFFSET, -BIG_SIZE);
            startLocs[MED] = new Vector2(Game1.TITLE_OFFSET, -MED_SIZE);
            startLocs[SML] = new Vector2(screenWidth - Game1.TITLE_OFFSET * 2, -SML_SIZE);

            //Loop through the length of the stars
            for (int i = 0; i < stars.Length; i++)
            {
                //Add a new star into the stars array
                stars[i] = new Star(starImgs[i], startLocs[i]);
            }
        }

        //Pre: none
        //Post: returns the integer corresponding to the game
        //Desc: returns the integer corresponding to the game, to be used by the driver
        public override int GetType()
        {
            //Return that this game is the catcher game
            return Game1.CATCHER;
        }

        //Pre: player is the player
        //Post: none
        //Desc: resets necessary elements of the catcher to prepare the game for the next wave of stars
        private void ResetCatcher(Player player) 
        {
            //Reset the selector
            selector.ResetClick();

            //Reset the player
            player.Setup(GetType(), screenWidth, screenHeight);

            //Reset whether or not the player change their speed
            hasChangedSpeed = false;

            //Loop through the stars to reactivate them
            for (int i = 0; i < stars.Length; i++) 
            {
                //Reactivate the star
                stars[i].Reactivate(startLocs[i]);
            }

            //Remove all the asteroids from the last wave
            asteroids.Clear();

            //Resets the wait timer
            waitTimer.ResetTimer(false);
        }

        //Pre: none
        //Post: none
        //Desc: loads in a new asteroid into the list
        private void LoadAsteroid()
        {
            //Check if the asteroid timer is not active
            if (!asteroidTimer.IsActive())
            {
                //Create the temporary asteroid position, direction, and asteroid
                Vector2 animPos = new Vector2(0, 0);
                int dir = 0;
                CatcherAsteroid temp = new CatcherAsteroid(asteroidImg, blankTexture, animPos, ASTEROID_SPEED, dir);

                //Retrieve the dimensions of the asteroid
                int width = temp.GetWidth();
                int height = temp.GetHeight();

                //Calculate a random integer representing the x position
                int xPos = Game1.rng.Next(LEFT, RIGHT);

                //Check if the xPos is to the left
                if (xPos == LEFT)
                {
                    //Setup the x position and set the asteroid moving right
                    animPos.X = -width;
                    dir = RIGHT;
                }
                else
                {
                    //Setup the x position and set the asteroid moving left
                    animPos.X = screenWidth;
                    dir = LEFT;
                }

                //Calculate the y position
                animPos.Y = Game1.rng.Next(0, selector.GetTop() - height + 1);
                
                //Add an asteroid to the list and reset the asteroid spawn timer
                asteroids.Add(new CatcherAsteroid(asteroidImg, blankTexture, animPos, ASTEROID_SPEED, dir));
                asteroidTimer.ResetTimer(true);
            }
        }

        //Pre: gameTime is the number of time passed in the game, player is the player, kb and prevKb are the keyboard input states, mouse and prevMouse are the mouse input states
        //Post: none
        //Desc: updates the star catcher game by updating timers, spawning asteroids, and detecting collision
        public override void UpdateGame(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            //Updates the base elements of the game
            base.UpdateGame(gameTime, player, kb, prevKb, mouse, prevMouse);

            //Updates the timers
            asteroidTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            waitTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Updates the selector
            selector.Update(mouse, prevMouse);

            //Check if the player has clicked the button to select their speed and direction
            if (selector.HasClicked())
            {
                //Check if the player's speed has not yet been changed
                if (!hasChangedSpeed)
                {
                    //Play the button hit sound
                    gameSnds[BTN].CreateInstance().Play();

                    //Update the player's x speed and direction and set the speed change boolean to true
                    player.ChangeXSpeed(selector.GetSpeed(), selector.GetDir());
                    hasChangedSpeed = true;

                    //Start the star collision wait timer
                    waitTimer.ResetTimer(true);
                }

                //Update the player
                player.Update(gameTime, GetType(), screenWidth, screenHeight, kb, prevKb, mouse, prevMouse);

                //Loop through the stars
                foreach (Star star in stars)
                {
                    //Check if the star is active
                    if (star.IsActive())
                    {
                        //Update the star
                        star.Update(gameTime);
                    }
                }
            }

            //Load an asteroid
            LoadAsteroid();

            //Loop through the asteroids
            foreach (CatcherAsteroid asteroid in asteroids)
            {
                //Update the asteroid
                asteroid.Update(gameTime);
            }

            //Check collision between objects and the player
            HasCollided(player);
        }

        //Pre: player is the player
        //Post: none
        //Desc: checks collision between the player and space objects, and the space objects and the edge of the screen
        public override void HasCollided(Player player)
        {
            //Store the player and star distance from an asteroid
            float playerDist;
            float starDist;

            //Check player collision with the screen
            if (player.GetRec().Bottom <= 0 || (!stars[BIG].IsActive() && !stars[MED].IsActive() && !stars[SML].IsActive()))
            {
                //Reset the catcher
                ResetCatcher(player);
            }
            else if (player.GetRec().Right >= screenWidth)
            {
                //Change the player's direction to left and play the bounce sound
                player.ChangeDir(X, LEFT);
                gameSnds[BOUNCE].CreateInstance().Play();
            }
            else if (player.GetRec().Left <= 0)
            {
                //Change the player's direction to right and play the bounce sound
                player.ChangeDir(X, RIGHT);
                gameSnds[BOUNCE].CreateInstance().Play();
            }
            else if (player.GetRec().Bottom >= screenHeight)
            {
                //Change the player's direction to moving up and play the bounce sound
                player.ChangeDir(Y, UP);
                gameSnds[BOUNCE].CreateInstance().Play();
            }

            //Loop through the asteroids to check player collision with them
            for (int i = 0; i < asteroids.Count; i++)
            {
                //Check if the asteroid collides with the screen
                if ((asteroids[i].GetRec().Left >= screenWidth && asteroids[i].GetDir() == RIGHT) || (asteroids[i].GetRec().Right <= 0 && asteroids[i].GetDir() == LEFT))
                {
                    //Remove the asteroid
                    asteroids.RemoveAt(i);
                }
                else
                {
                    //Calculate the player distance from the asteroid
                    playerDist = (float)Math.Sqrt(Math.Pow(asteroids[i].GetHitBox().Center.X - player.GetRec().Center.X, 2) + Math.Pow(asteroids[i].GetHitBox().Center.Y - player.GetRec().Center.Y, 2));

                    //Check if the player distance is less than or equal to the sum of the radiuses of the player and the asteroid
                    if (playerDist <= player.GetRadius() + asteroids[i].GetRadius())
                    {
                        //Calculate the resultant vector and add it to the player's position
                        Vector2 resultant = Vector2.Normalize(player.GetRec().Center.ToVector2() - asteroids[i].GetHitBox().Center.ToVector2());
                        resultant *= player.GetRadius() + asteroids[i].GetRadius() - playerDist;
                        player.AddToPos(resultant);

                        //Check the sign of the x coordinate of the resultant
                        if (Math.Sign(resultant.X) < 0)
                        {
                            //Change the player's direction to left
                            player.ChangeDir(X, LEFT);
                        }
                        else
                        {
                            //Change the player's direction to right
                            player.ChangeDir(X, RIGHT);
                        }

                        //Check the sign of the y coordinate of the resultant
                        if (Math.Sign(resultant.Y) < 0)
                        {
                            //Change the player's direction to up on the screen
                            player.ChangeDir(Y, UP);
                        }
                        else
                        {
                            //Change the player's direction to down on the screen
                            player.ChangeDir(Y, DOWN);
                        }

                        //Play the bounce sound effect
                        gameSnds[BOUNCE].CreateInstance().Play();
                    }
                }
            }

            //Loop through the stars
            for (int i = 0; i < stars.Length; i++)
            {
                //Check if the star is active
                if (stars[i].IsActive())
                {
                    //Check the star's collision
                    if (stars[i].GetRec().Intersects(player.GetRec()))
                    {
                        //Add the corresponding points to the total points
                        points += starPoints[i];

                        //Deactivate the star
                        stars[i].Deactivate();

                        //Play the star catch sound
                        gameSnds[CATCH].CreateInstance().Play();
                    }
                    else if (stars[i].GetRec().Right >= screenWidth)
                    {
                        //Change the star's direction to left and play the bounce sound
                        stars[i].ChangeDir(X, LEFT);
                        gameSnds[BOUNCE].CreateInstance().Play();
                    }
                    else if (stars[i].GetRec().Left <= 0)
                    {
                        //Change the star's direction to right and play the bounce sound
                        stars[i].ChangeDir(X, RIGHT);
                        gameSnds[BOUNCE].CreateInstance().Play();
                    }
                    else if (stars[i].GetRec().Bottom <= 0)
                    {
                        //Deactivate the star
                        stars[i].Deactivate();
                    }
                    else if (stars[i].GetRec().Bottom >= screenHeight)
                    {
                        //Change the star's direction to down and play the bounce sound
                        stars[i].ChangeDir(Y, DOWN);
                        gameSnds[BOUNCE].CreateInstance().Play();
                    }

                    //Check if the wait timer is not active
                    if (!waitTimer.IsActive())
                    {
                        //Loop through the asteroids to check their collision with the star
                        for (int j = 0; j < asteroids.Count; j++)
                        {
                            //Calculate the star distance from the asteroid
                            starDist = (float)Math.Sqrt(Math.Pow(asteroids[j].GetHitBox().Center.X - stars[i].GetRec().Center.X, 2) + Math.Pow(asteroids[j].GetHitBox().Center.Y - stars[i].GetRec().Center.Y, 2));

                            //Check if the star distance is less than or equal to the sum of the radiuses of the star and the asteroid
                            if (starDist <= stars[i].GetRadius() + asteroids[j].GetRadius())
                            {
                                //Calculate the resultant vector and add it to the star's position
                                Vector2 resultant = Vector2.Normalize(stars[i].GetRec().Center.ToVector2() - asteroids[j].GetHitBox().Center.ToVector2());
                                resultant *= stars[i].GetRadius() + asteroids[j].GetRadius() - starDist;
                                stars[i].AddToPos(resultant);

                                //Check the sign of the x coordinate of the resultant
                                if (Math.Sign(resultant.X) < 0)
                                {
                                    //Change the star's direction to left
                                    stars[i].ChangeDir(X, LEFT);
                                }
                                else
                                {
                                    //Change the star's direction to right
                                    stars[i].ChangeDir(X, RIGHT);
                                }

                                //Check the sign of the y coordinate of the resultant
                                if (Math.Sign(resultant.Y) < 0)
                                {
                                    //Change the star's direction to down on the screen (visually up)
                                    stars[i].ChangeDir(Y, DOWN);
                                }
                                else
                                {
                                    //Change the star's direction to up on the screen (visually down)
                                    stars[i].ChangeDir(Y, UP);
                                }

                                //Play the bounce sound effect
                                gameSnds[BOUNCE].CreateInstance().Play();
                            }
                        }
                    }
                }
            }
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images, player is the player
        //Post: none
        //Desc: draws all elements of the star catchers game, including text, space objects, and the player
        public override void Draw(SpriteBatch spriteBatch, Player player)
        {
            //Draw the background image
            spriteBatch.Draw(bgImg, bgRec, Color.White);

            //Loop through each asteroid to draw it
            foreach (CatcherAsteroid asteroid in asteroids)
            {
                //Draw the asteroid
                asteroid.Draw(spriteBatch);
            }

            //Loop through each star to draw it
            foreach (Star star in stars)
            {
                //Draw the star
                star.Draw(spriteBatch);
            }

            //Draw the player
            player.Draw(spriteBatch, GetType());

            //Draw the selector
            selector.Draw(spriteBatch);

            //Draw the base elements of the game
            base.Draw(spriteBatch, player);
        }
    }
}
