/* Author: Matthew Kamenetsky
 * File name: WhackAnAsteroid.cs
 * Project name: PASS3_ICS4U
 * Creation date: June 08, 2023
 * Modified date: June 12, 2023
 * Description: The WhackAnAsteroid class is responsible for updating the gameplay of the Whack-A-'Roid minigame. It manages all of the asteroids and how the player interacts with them, as it updates and draws them both.  
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
    class WhackAnAsteroid : Minigame
    {
        //Store the index of the missclick sound and spawn sound
        public const int MISS_CLICK = 0;
        public const int SPAWN = 3;

        //Store location indexes
        private const int ONE = 0;
        private const int TWO = 1;
        private const int THREE = 2;
        private const int FOUR = 3;
        private const int FIVE = 4;

        //Store image indexes
        public const int CIRCLE = 0;
        public const int ASTEROID = 1;
        public const int POOF = 2;
        public const int EXPLOSION = 3;

        //Store a third of the game time
        private const int TIME_INCREMENT = 20000;

        //Store the staying timer time indexes
        private const int EARLY_MAX_STAY = 825;
        private const int EARLY_MIN_STAY = 750;
        private const int MID_MIN_STAY = 675;
        private const int HIGH_MIN_STAY = 600;

        //Store changing timer time indexes
        private const int EARLY_MAX_CHANGE = 950;
        private const int EARLY_MIN_CHANGE = 825;
        private const int MID_MIN_CHANGE = 700;
        private const int HIGH_MIN_CHANGE = 575;

        //Store the mouse and previous mouse
        private MouseState mouse;
        private MouseState prevMouse;

        //Store the gameboard and the corresponding integers for the asteroid locations
        private Vector2[,] gameBoard = new Vector2[5, 5];
        private int[] gameXs = new int[5];
        private int[] gameYs = new int[5];

        //Store the array of asteroids
        private WhackerAsteroid[] asteroids = new WhackerAsteroid[25];

        //Store the current asteroid
        private WhackerAsteroid curAsteroid;

        //Store the asteroid images
        private Texture2D[] asteroidImgs;

        //Store the change time and timer
        private double changeTime;
        private Timer changeTimer;

        //Store the stay time and timer
        private double stayTime;
        private Timer stayTimer;

        //Store the time limits of the change time
        private double minChange;
        private double maxChange;

        //Store the time limits of the stay time
        private double minStay;
        private double maxStay;

        //Store point related data
        private int hitPts = 15;
        private int explodePts = 10;
        private int missClickPts = 5;

        //Pre: fonts are the game fonts, bg is the game's background image, asteroidImgs are the asteroid's images, gameSnds are the game's sounds, screenWidth and screenHeight are the screen dimensions
        //Post: none
        //Desc: sets up the whack-a-'roid game with necessary values
        public WhackAnAsteroid(SpriteFont[] fonts, Texture2D bg, Texture2D[] asteroidImgs, SoundEffect[] gameSnds, int screenWidth, int screenHeight) : base(fonts, bg, gameSnds, screenWidth, screenHeight)
        {
            //Store the asteroid images
            this.asteroidImgs = asteroidImgs;

            //Set up the change time and timer
            changeTime = Game1.rng.Next(EARLY_MIN_CHANGE, EARLY_MAX_CHANGE);
            changeTimer = new Timer(changeTime, true);

            //Setup the stay timer
            stayTimer = new Timer(changeTime, false);

            //Setup the x locations of each column
            gameXs[ONE] = Game1.SIDE_OFFSET + asteroidImgs[CIRCLE].Width;
            gameXs[THREE] = screenWidth / 2 - asteroidImgs[CIRCLE].Width / 2;
            gameXs[TWO] = (gameXs[ONE] + gameXs[THREE]) / 2;
            gameXs[FIVE] = screenWidth - Game1.SIDE_OFFSET - asteroidImgs[CIRCLE].Width * 2;
            gameXs[FOUR]= (gameXs[THREE] + gameXs[FIVE]) / 2;

            //Setup the y location of the first row
            gameYs[ONE] = Game1.TITLE_OFFSET + (int)(Game1.SIDE_OFFSET * 0.75);

            //Loop through the remaining rows
            for (int i = TWO; i < gameYs.Length; i++)
            {
                //Setup the y location of every row
                gameYs[i] = gameYs[i - 1] + asteroidImgs[ASTEROID].Height + Game1.TITLE_OFFSET + (int)(Game1.SIDE_OFFSET * 0.75);
            }

            //Load the game board and the asteroids
            LoadBoard();
        }

        //Pre: none
        //Post: returns the integer corresponding to the game
        //Desc: returns the integer corresponding to the game, to be used by the driver
        public override int GetType()
        {
            //Return that the game being played is the whacker
            return Game1.WHACKER;
        }

        //Pre: none
        //Post: none
        //Desc: loads the gameboard locations, then fills up the array of asteroids
        private void LoadBoard() 
        {
            //Loop through the rows
            for (int i = 0; i <= FIVE; i++) 
            {
                //Loop through the columns
                for (int j = 0; j <= FIVE; j++) 
                {
                    //Set the gameboard location
                    gameBoard[i, j] = new Vector2(gameXs[j], gameYs[i]);
                }
            }

            //Loop through the asteroid array's length
            for (int i = 0; i < asteroids.Length; i++) 
            {
                //Calculate the row and column of the asteroid
                int row = i / gameBoard.GetLength(1);
                int col = i % gameBoard.GetLength(0);

                //Setup the asteroid
                asteroids[i] = new WhackerAsteroid(asteroidImgs, gameBoard[row, col]);
            }
        }

        //Pre: gameTime is the number of time passed in the game, player is the player, kb and prevKb are the keyboard input states, mouse and prevMouse are the mouse input states
        //Post: none
        //Desc: updates the whack-a-'roid game by updating timers, spawning asteroids, and detecting collision
        public override void UpdateGame(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            //Store the mouse states
            this.mouse = mouse;
            this.prevMouse = prevMouse;

            //Update the base elements of the game
            base.UpdateGame(gameTime, player, kb, prevKb, mouse, prevMouse);

            //Update the timers
            changeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            stayTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Check the time played in order to setup minimum and maximum values of other timers
            CheckTime();

            //Update the player
            player.Update(gameTime, GetType(), screenWidth, screenHeight, kb, prevKb, mouse, prevMouse);

            //Check if the previous asteroid's staying up timer is over
            if (!stayTimer.IsActive())
            {
                //Spawn a new asteroid
                SpawnAsteroid();
            }

            //Check if the asteroid is not null
            if (curAsteroid != null)
            {
                //Update the asteroid
                curAsteroid.Update(gameTime);

                //Check if the asteroid selection process is ongoing
                if (!changeTimer.IsActive())
                {
                    //Check collision
                    HasCollided(player);
                }
            }
        }

        //Pre: none
        //Post: none
        //Desc: checks the time passed to determine the limist of the change and stay timers
        private void CheckTime() 
        {
            //Check the time passed
            if (gameTimer.GetTimeRemaining() >= TIME_INCREMENT)
            {
                //Set the limits of the min and max times to the early ones
                minChange = EARLY_MIN_CHANGE;
                maxChange = EARLY_MAX_CHANGE;
                minStay = EARLY_MIN_STAY;
                maxStay = EARLY_MAX_STAY;
            }
            else if (gameTimer.GetTimeRemaining() >= TIME_INCREMENT * 2)
            {
                //Set the limits of the min and max times to the mid ones
                minChange = MID_MIN_CHANGE;
                maxChange = EARLY_MIN_CHANGE;
                minStay = MID_MIN_STAY;
                maxStay = EARLY_MIN_STAY;
            }
            else
            {
                //Set the limits of the min and max times to the high ones
                minChange = HIGH_MIN_CHANGE;
                maxChange = MID_MIN_CHANGE;
                minStay = HIGH_MIN_STAY;
                maxStay = MID_MIN_STAY;
            }
        }

        //Pre: none
        //Post: none
        //Desc: spawns an asteroid
        private void SpawnAsteroid() 
        {
            //Check if the change timer is not active
            if (!changeTimer.IsActive())
            {
                //Calculate a random asteroid
                int rand = Game1.rng.Next(0, asteroids.Length);

                //Calculate the stay time
                stayTime = Game1.rng.Next((int)minStay, (int)maxStay + 1);

                //Check if the current asteroid is null
                if (curAsteroid == null)
                {
                    //Set the current asteroid to the one at the random index and activate it
                    curAsteroid = asteroids[rand];
                    curAsteroid.Activate();

                    //Play the spawn sound and reset the stay timer
                    gameSnds[SPAWN].CreateInstance().Play();
                    stayTimer = new Timer(stayTime, true);
                }
                else
                {
                    //Check if the current asteroid is inactive
                    if (curAsteroid.GetActivity() == WhackerAsteroid.INACTIVE)
                    {
                        //Loop until the current asteroid is not the same as the one at the random index
                        while (curAsteroid == asteroids[rand])
                        {
                            //Calculate a new random index
                            rand = Game1.rng.Next(0, asteroids.Length);
                        }

                        //Set the current asteroid to the one at the random index and activate it
                        curAsteroid = asteroids[rand];
                        curAsteroid.Activate();

                        //Play the spawn sound and reset the stay timer
                        gameSnds[SPAWN].CreateInstance().Play();
                        stayTimer = new Timer(stayTime, true);
                    }
                }
            }
        }

        //Pre: player is the player
        //Post: none
        //Desc: checks collision between the player and asteroids
        public override void HasCollided(Player player)
        {
            //Check player input to determine how to proceed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Loop through the asteroids
                for (int i = 0; i < asteroids.Length; i++)
                {
                    //Check if an asteroid contains the mouse
                    if (asteroids[i].GetRec().Contains(mouse.Position))
                    {
                        //Check if the current asteroid was clicked
                        if (asteroids[i].Equals(curAsteroid))
                        {
                            //Kill the asteroid and play the poof sound
                            curAsteroid.Kill();
                            gameSnds[POOF].CreateInstance().Play();

                            //Add the hit points
                            points += hitPts;

                            //Calculate the change time and create the new change timer
                            changeTime = Game1.rng.Next((int)minChange, (int)maxChange + 1);
                            changeTimer = new Timer(changeTime, true);
                        }
                        else 
                        {
                            //Play the misclick sound and decrease the points by the misclick amount
                            gameSnds[MISS_CLICK].CreateInstance().Play();
                            points = Math.Max(0, points - missClickPts);
                        }
                    }
                }
            }

            //Check if the stay timer is not active
            if (!stayTimer.IsActive())
            {
                //Check the activity of the asteroid
                if (curAsteroid.GetActivity() == WhackerAsteroid.ACTIVE)
                {
                    //Explode the asteroid, play the explosion sound, and decrease the player's points
                    curAsteroid.Explode();
                    gameSnds[WhackerAsteroid.EXPLODE].CreateInstance().Play();
                    points = Math.Max(0, points - explodePts);

                    //Calculate the change time and create the new change timer
                    changeTime = Game1.rng.Next((int)minChange, (int)maxChange + 1);
                    changeTimer = new Timer(changeTime, true);
                }
            }
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images, player is the player
        //Post: none
        //Desc: draws all elements of the whack-a-'roid game, including text, asteroids, and the player
        public override void Draw(SpriteBatch spriteBatch, Player player)
        {
            //Draw the background image
            spriteBatch.Draw(bgImg, bgRec, Color.White);

            //Loop through each asteroid
            foreach (WhackerAsteroid asteroid in asteroids) 
            {
                //Draw the asteroid
                asteroid.Draw(spriteBatch);
            }

            //Draw the player
            player.Draw(spriteBatch, GetType());

            //Draw the base elements of the game
            base.Draw(spriteBatch, player);
        }
    }
}
