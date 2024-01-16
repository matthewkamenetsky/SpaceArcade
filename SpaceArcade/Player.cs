/* Author: Matthew Kamenetsky
 * File name: Player.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 13, 2023
 * Modified date: June 12, 2023
 * Description: the player class is responsible for all elements of the player, including updating them, drawing them, and returning necessary values
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
    class Player
    {
        //Store flame indexes of the player for the runner game
        public const int FLAME = 0;
        public const int FIZZLE = 1;
        
        //Store tolerances for the flame images of the player for the runner game
        private const int FLAME_TOL = 5;
        private const int FIZZLE_TOL = 4;

        //Store the bottom of the screen tolerance for the runner game
        private const int BOT_TOL = 120;

        //Store the speed boost after changing directions for the catcher game
        private const float SPEED_BOOST = 0.5f;

        //Store the player images, rectangle, position, speeds, current speed, and soundeffects
        private Texture2D[] playerImgs = new Texture2D[5];
        private Rectangle playerRec;
        private Vector2 pos;
        private float[] speeds = new float[] { 5.25f, 4.15f , 5f, 0f};
        private float speed;
        private SoundEffect[] playerSnds;

        //Store the player's username and password
        private string[] names = new string[2];
        
        //Store the players high highScores
        private int[] highScores;

        //Store the player's current scores
        private int[] curScores = new int[] { 0, 0, 0, 0, 0, 0 };

        //Store player data related to the alien shooter
        private Texture2D bulletImg;
        private Timer fireTimer;
        private double fireTime = 333;
        private List<Bullet> bullets = new List<Bullet>();

        //Store player data related to the space runner
        private Animation flameAnim;
        private Texture2D[] flameImgs;
        private Vector2 flamePos;
        private Rectangle fizzleRec;
        private double flameTime = 200;
        private Timer flameTimer;
        private Timer[] dirTimers = new Timer[2];

        //Store player data related to the star catcher
        private float xSpeed = 0f;
        private int[] dir = new int[] { StarCatcher.RIGHT, StarCatcher.UP };
        private float diameter;
        private float[] speedBoosts = new float[2] { 0, 0 };
        private double boostTime = 750;
        private Timer boostTimer;

        //Pre: names are the player's user name and password, highScores are the player's high scores, playerImgs are the player's images, bulletImg is the player's bullet image, flameImgs are the player's flame images, playerSnds are the player's sounds
        //Post: none
        //Desc: creates a player with necessary values
        public Player(string[] names, int[] highScores, Texture2D[] playerImgs, Texture2D bulletImg, Texture2D[] flameImgs, SoundEffect[] playerSnds)
        {
            //Setup the base elements of the player
            this.names = names;
            this.highScores = highScores;
            this.playerImgs = playerImgs;
            this.playerSnds = playerSnds;

            //Store the alien shooter elements of the player
            this.bulletImg = bulletImg;
            fireTimer = new Timer(fireTime, false);

            //Store the space run elements of the player
            this.flameImgs = flameImgs;
            flameTimer = new Timer(flameTime, false);
            dirTimers[FLAME] = new Timer(Timer.INFINITE_TIMER, false);
            dirTimers[FIZZLE] = new Timer(Timer.INFINITE_TIMER, false);

            //Store the star catchers elements of the game
            boostTimer = new Timer(boostTime, false);
        }

        //Pre: type is the type of name being retrieved
        //Post: returns the name at the type of name
        //Desc: returns the name at the type of name
        public string GetName(int type)
        {
            //Return the name
            return names[type];
        }

        //Pre: gameType is the type of game
        //Post: returns the high score at the gametype
        //Desc: returns the high score at the gametype
        public int GetPoints(int gameType)
        {
            //Return the highscore
            return highScores[gameType];
        }

        //Pre: gameType is the type of game
        //Post: returns the current score at the gametype
        //Desc: returns the current score at the gametype
        public int GetCurPoints(int gameType)
        {
            //Return the current score
            return curScores[gameType];
        }

        //Pre: none
        //Post: returns the player's position
        //Desc: returns the player's position
        public Vector2 GetPos()
        {
            //Return the player's position
            return pos;
        }

        //Pre: none
        //Post: returns the player's rectangle
        //Desc: returns the player's rectangle
        public Rectangle GetRec()
        {
            //Return the player's rectangle
            return playerRec;
        }

        //Pre: none
        //Post: returns the player's bullets
        //Desc: returns the player's bullets
        public List<Bullet> GetBullets()
        {
            //Return the bullets
            return bullets;
        }

        //Pre: type is the type of direction
        //Post: returns the time spent going in one direction
        //Desc: returns the time spent going in one direction
        public double GetDirTime(int type) 
        {
            //Return the time spent going in one direction
            return dirTimers[type].GetTimePassed();
        }

        //Pre: none
        //Post: returns the radius of the player
        //Desc: returns the radius of the player
        public float GetRadius()
        {
            //Return the player's radius
            return diameter / 2;
        }

        //Pre: toAdd is the vector being added to the position
        //Post: none
        //Desc: adds a vector to the position of the player to offset collision
        public void AddToPos(Vector2 toAdd) 
        {
            //Add the vector to the player's position and update the player's rectangle
            pos += toAdd;
            playerRec.X = (int)pos.X;
            playerRec.Y = (int)pos.Y;
        }

        //Pre: speed is the calculated speed, dir is the calculated direction
        //Post: none
        //Desc: sets up the player's x speed and x direction for the catcher game
        public void ChangeXSpeed(float speed, int dir) 
        {
            //Setup the xSpeed and x direction
            xSpeed = speed;
            this.dir[StarCatcher.X] = dir;
        }

        //Pre: type is the coordinate of direction being change, sign is the sign of direction
        //Post: none
        //Desc: changes the direction of the coordinate
        public void ChangeDir(int type, int sign) 
        {
            //Check that the direction of the coordinate is not the same as the sign of direction
            if (dir[type] != sign) 
            {
                //Set the direction of the coordinate to the sign of direction
                dir[type] = sign;
            }

            //Add the speed boost to the speed boosts at the coordinate and reset the speedboost timer
            speedBoosts[type] += SPEED_BOOST;
            boostTimer.ResetTimer(true);
        }

        //Pre: gameType is the type of game that the points are saved to, points are the points in that game
        //Post: none
        //Desc: sets the current points at the gameType to the inputted points
        public void SetPoints(int gameType, int points)
        {
            //Set the current points at the gameType to the inputted points
            curScores[gameType] = points;
        }

        //Pre: none
        //Post: none
        //Desc: checks if the current score is greater than the high score at that index
        public void SavePoints() 
        {
            //Loop through the highscores
            for (int i = 0; i < highScores.Length; i++) 
            {
                //Check if the current score is greater or equal to the high score at that index
                if (curScores[i] >= highScores[i]) 
                {
                    //Set the highscore at the index to the current score at the index
                    highScores[i] = curScores[i];
                }
            }
        }

        //Pre: idx1 is the index of the first game played, idx2 is the index of the second game played, idx3 is the index of the last game played;
        //Post: none
        //Desc: saves the tri scores to the total of the games inputted
        public void SaveTri(int idx1, int idx2, int idx3) 
        {
            //Calculate the current tri score
            curScores[Game1.TRI] = curScores[idx1] + curScores[idx2] + curScores[idx3];
        }

        //Pre: none
        //Post: none
        //Desc: saves the quad scores to the total of the current games player
        public void SaveQuad() 
        {
            //Calculate the quad score
            curScores[Game1.QUAD] = curScores[Game1.SHOOTER] + curScores[Game1.RUNNER] + curScores[Game1.CATCHER] + curScores[Game1.WHACKER];
        }

        //Pre: gameType is the type of game being setup, width and height are the screen dimensions
        //Post: none
        //Desc: sets up the player for the next game by setting up necessary values
        public void Setup(int gameType, int width, int height)
        {
            //Set the speed to the one at the gameType
            speed = speeds[gameType];

            //Check the gameType to determine what to setup
            switch (gameType)
            {
                case Game1.SHOOTER:
                    //Clear the players bullets
                    bullets.Clear();

                    //Setup the player's rectangle
                    playerRec = new Rectangle(width / 2 - playerImgs[gameType].Width / 2, height - playerImgs[gameType].Height, playerImgs[gameType].Width, playerImgs[gameType].Height);  
                    
                    //Setup the player's rate of fire
                    fireTimer = new Timer(fireTime, false);
                    break;
                case Game1.RUNNER:
                    //Setup the player's rectangle, and other related parts to the player's visibility
                    playerRec = new Rectangle(playerImgs[gameType].Width, height - playerImgs[gameType].Height * 2, playerImgs[gameType].Width, playerImgs[gameType].Height);
                    flameAnim = new Animation(flameImgs[FLAME], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 3, flamePos, 1f, false);
                    flamePos.X = playerRec.Center.X - flameAnim.destRec.Width / 2 - FLAME_TOL;
                    flamePos.Y = playerRec.Bottom;
                    flameAnim.destRec.X = (int)flamePos.X;
                    flameAnim.destRec.Y = (int)flamePos.Y;
                    fizzleRec = new Rectangle((int)flamePos.X + FIZZLE_TOL, (int)flamePos.Y, flameImgs[FIZZLE].Width, flameImgs[FIZZLE].Height);
                    break;
                case Game1.CATCHER:
                    //Setup the player's rectangle
                    playerRec = new Rectangle((width - playerImgs[gameType].Width) / 2, height - playerImgs[gameType].Height - Game1.TITLE_OFFSET / 2, playerImgs[gameType].Width, playerImgs[gameType].Height);
                    
                    //Reset the player's direction and speedboosts
                    dir = new int[] { StarCatcher.RIGHT, StarCatcher.UP };
                    speedBoosts = new float[] { 0, 0 };
                    break;
                case Game1.WHACKER:
                    //Setup the player's rectangle
                    playerRec = new Rectangle(0, 0, playerImgs[gameType].Width, playerImgs[gameType].Height);
                    break;
            }

            //Set the position of the player to the position of the player's rectangle
            pos.X = playerRec.X;
            pos.Y = playerRec.Y;
        }

        //Pre: gameTime is the number of time passed in the game, gameType is the type of game, width and height are the screen dimensions, kbState and prevKb are the keyboard input states, mouse and prevMouse are the mouse input states
        //Post: none
        //Desc: updates the player based on the current gameType
        public void Update(GameTime gameTime, int gameType, int width, int height, KeyboardState kbState, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            //Check the gameType to determine how to update the player
            switch (gameType)
            {
                case Game1.SHOOTER:
                    //Update the shooter game
                    UpdateShooter(gameTime, width, height, kbState, prevKb);
                    break;
                case Game1.RUNNER:
                    //Update the runner game
                    UpdateRunner(gameTime, kbState, height);
                    break;
                case Game1.CATCHER:
                    //Update the catcher game
                    UpdateCatcher(gameTime, mouse, prevMouse);
                    break;
                case Game1.WHACKER:
                    //Update the whacker game
                    UpdateWhacker(mouse, prevMouse);
                    break;
            }
        }

        //Pre: gameTime is the number of time passed in the game, width and height are the screen dimensions, kbState and prevKb are the keyboard input states
        //Post: none
        //Desc: updates the player in the shooter game, including the player's movement and shooting
        private void UpdateShooter(GameTime gameTime, int width, int height, KeyboardState kbState, KeyboardState prevKb)
        {
            //Updates the rate of fire timer
            fireTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Check user input to determine movement direction
            if (kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.A))
            {
                //Check's if the player can move more to the left without being offscreen
                if (pos.X > 0)
                {
                    //Moves the player left by their speed
                    pos.X -= speed;
                }
            }
            else if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
            {
                //Check's if the player can move more to the right without being offscreen
                if (pos.X + playerImgs[Game1.SHOOTER].Width < width)
                {
                    //Moves the player right by their speed
                    pos.X += speed;
                }
            }

            //Sets the new position of the player's rectangle
            playerRec.X = (int)pos.X;

            //Allows the player to shoot
            Shoot(kbState, prevKb);
        }

        //Pre: kbState is the current keyboard state, prevKb is the previous keyboard state
        //Post: none
        //Desc: allows the player to shoot an arrow by checking the reload timer and user input
        private void Shoot(KeyboardState kbState, KeyboardState prevKb)
        {
            //Check's if the player has reloaded
            if (!fireTimer.IsActive())
            {
                //Check's if the user pressed the space key
                if (kbState.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
                {
                    //Add an arrow to the player's bullets and set its position
                    bullets.Add(new Bullet(bulletImg, pos, 6.25f));
                    bullets.Last().SetPosition(playerRec, AlienShooter.PLAYER);

                    //Play the shooting sound effect
                    SoundEffectInstance shoot = playerSnds[Game1.SHOOTER].CreateInstance();
                    shoot.Volume = 0.4f;
                    shoot.Play();

                    //Reset the reload timer
                    fireTimer.ResetTimer(true);
                }
            }
        }

        //Pre: gameTime is the number of time passed in the game, kb is the keyboard's input state, height is the screen's height
        //Post: none
        //Desc: updates the player in the runner game
        private void UpdateRunner(GameTime gameTime, KeyboardState kb, int height)
        {
            //Update the timers
            flameTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            dirTimers[FLAME].Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            dirTimers[FIZZLE].Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Check the player's input
            if (kb.IsKeyDown(Keys.Space))
            {
                //Check if the upwards direction timer is not yet active
                if (!dirTimers[FLAME].IsActive()) 
                {
                    //Reset the downwards direction timer to false and reset the upwards direction timer to true
                    dirTimers[FIZZLE].ResetTimer(false);
                    dirTimers[FLAME].ResetTimer(true);
                }

                //Check if the flame sound timer is not active
                if (!flameTimer.IsActive()) 
                {
                    //Play the flame sound and reset the flame sound timer
                    playerSnds[Game1.RUNNER].CreateInstance().Play();
                    flameTimer.ResetTimer(true);
                }

                //Move the y position upwards
                pos.Y -= speed;

                //Animate the player's flame
                flameAnim.isAnimating = true;
                flameAnim.Update(gameTime);
            }
            else
            {
                //Check if the downwards direction timer is not yet active
                if (!dirTimers[FIZZLE].IsActive())
                {
                    //Reset the upwards direction timer to false and reset the downwards direction timer to true
                    dirTimers[FLAME].ResetTimer(false);
                    dirTimers[FIZZLE].ResetTimer(true);
                }

                //Deactivate the flame sound timer
                flameTimer.Deactivate();

                //Move the player down
                pos.Y += speed * 1.2f;

                //Stop animating the flame
                flameAnim.isAnimating = false;
            }

            //Place a bounds on the player's y position and update the player's rectangle
            pos.Y = MathHelper.Clamp(pos.Y, 0, height - BOT_TOL - playerImgs[Game1.SHOOTER].Height);
            playerRec.Y = (int)pos.Y;

            //Set the animation rectangle and fizzle rectangle to the bottom of the player's rectangle
            flameAnim.destRec.Y = playerRec.Bottom;
            fizzleRec.Y = playerRec.Bottom;
        }

        //Pre: gameTime is the number of time passed in the game, mouse and prevMouse are the mouse input states
        //Post: none
        //Desc: updates the player in the catcher game
        private void UpdateCatcher(GameTime gameTime, MouseState mouse, MouseState prevMouse) 
        {
            //Updates the boost timer
            boostTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Check if the boost timer is not active
            if (!boostTimer.IsActive()) 
            {
                //Loop through the speedboosts
                for (int i = 0; i < speedBoosts.Length; i++) 
                {
                    //Set the speedboosts to 0
                    speedBoosts[i] = 0;
                }
            }

            //Calculate the players x and y position and update the player's rectangle
            pos.X += dir[StarCatcher.X] * (xSpeed + speedBoosts[StarCatcher.X]);
            pos.Y -= dir[StarCatcher.Y] * (speed + speedBoosts[StarCatcher.Y]);
            playerRec.X = (int)pos.X;
            playerRec.Y = (int)pos.Y;

            //Calculate the player's diameter
            diameter = Math.Min(playerImgs[Game1.CATCHER].Width, playerImgs[Game1.CATCHER].Height);
        }

        //Pre: mouse and prevMouse are the mouse input states
        //Post: none
        //Desc: updates the player in the whacker game
        private void UpdateWhacker(MouseState mouse, MouseState prevMouse) 
        {
            //Set the player's rectangle to the mouse's position
            playerRec.X = mouse.Position.X - playerRec.Width / 2;
            playerRec.Y = mouse.Position.Y - playerRec.Height / 2;
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images, gameType is the type of game being drawn
        //Post: none
        //Desc: draws the player and their elements
        public void Draw(SpriteBatch spriteBatch, int gameType)
        {
            //Draws the player
            spriteBatch.Draw(playerImgs[gameType], playerRec, Color.White);

            //Check if the runner is being played
            if (gameType == Game1.RUNNER)
            {
                //Check if the flame is animating
                if (flameAnim.isAnimating == true)
                {
                    //Draw the flame animation
                    flameAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
                }
                else
                {
                    //Draw the fizzle image
                    spriteBatch.Draw(flameImgs[FIZZLE], fizzleRec, Color.White);
                }
            }
        }
    }
}
