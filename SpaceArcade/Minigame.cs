/* Author: Matthew Kamenetsky
 * File name: Minigame.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 11, 2023
 * Modified date: June 12, 2023
 * Description: The Minigame class is the parent class for all minigames. It is responsible for setting up, updating, and drawing the base parts of each minigame 
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
    class Minigame
    {
        //Store the fonts
        protected SpriteFont[] fonts;

        //Store the soundeffects
        protected SoundEffect[] gameSnds;

        //Store the screen dimensions
        protected int screenWidth;
        protected int screenHeight;

        //Stores the background image and rectangle
        protected Texture2D bgImg;
        protected Rectangle bgRec;
        
        //Store the game length, timer, time text, and time text location
        protected double targetTime = 60000;
        protected Timer gameTimer;
        protected string timeText = "";
        protected Vector2 timeLoc;

        //Store points, score text, high score text, the score text location, and high score text location
        protected int points;
        protected string scoreText = "Score: ";
        protected string highText = "High Score: ";
        protected Vector2 scoreLoc = new Vector2(Game1.SIDE_OFFSET / 4, Game1.SIDE_OFFSET / 4);
        protected Vector2 highLoc = new Vector2(0, Game1.SIDE_OFFSET / 4);

        //Pre: fonts are the fonts used in game, bg is the background image, gameSnds are the in game sounds, screenWidth and screenHeight are the screen dimensions
        //Post: none
        //Desc: sets up the alien shooter game object with necessary values
        public Minigame(SpriteFont[] fonts, Texture2D bg, SoundEffect[] gameSnds, int screenWidth, int screenHeight) 
        {
            //Setup the fonts
            this.fonts = fonts;

            //Setup the screen dimensions
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            //Setup the background image and create its rectangle
            bgImg = bg;
            bgRec = new Rectangle(0, 0, bgImg.Width, bgImg.Height);

            //Setup the game timer and the time location
            gameTimer = new Timer(targetTime, true);
            timeLoc = new Vector2(0, Game1.SIDE_OFFSET / 4);

            //Set up the game sounds
            this.gameSnds = gameSnds;
        }

        //Pre: none
        //Post: returns the game's points
        //Desc: returns the game's points
        public int GetPts() 
        {
            //Return the game's points
            return points;
        }

        //Pre: none
        //Post: returns if the game is active
        //Desc: returns if the game is active
        public bool IsActive() 
        {
            //Return if the game is active
            return gameTimer.IsActive();
        }

        //Pre: none
        //Post: returns the game type
        //Desc: returns the game type
        public virtual int GetType() 
        {
            //Return the base game type
            return Game1.SHOOTER;
        }

        //Pre: gameTime is the number of time passed in the game, player is the player, kb and prevKb are the keyboard input states, mouse and prevMouse are the mouse input states
        //Post: none
        //Desc: updates the base elements of the minigame
        public virtual void UpdateGame(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse) 
        {
            //Update the timer
            gameTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Calculate the high score text and its location
            highText = "High Score: " + player.GetPoints(GetType());
            highLoc.X = screenWidth / 2 - fonts[Game1.BOLD].MeasureString(highText).X / 2;

            //Store the second and millisecond time
            string secTime = "";
            string millTime = "";

            //Check if the game has not finished
            if (!gameTimer.IsFinished())
            {
                //Calculate the second and millisecond time
                secTime = gameTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL).Substring(0, gameTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL).IndexOf('.') + 1).PadLeft(3, '0');
                millTime = gameTimer.GetTimeRemainingAsString(Timer.FORMAT_MIL).Substring(0, 2).PadRight(2, '0');
            }
            else 
            {
                //Set the second and millisecond time to finished times
                secTime = "00.";
                millTime = "00";
            }
            
            //Store the time text and create its x location
            timeText = "Time Left: " + secTime + millTime;
            timeLoc.X = screenWidth - fonts[Game1.BOLD].MeasureString(timeText).X - Game1.SIDE_OFFSET / 4;
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images
        //Post: none
        //Desc: draws the base elements of the minigame
        public virtual void Draw(SpriteBatch spriteBatch, Player player)
        {
            //Draw the score, high score, and time remaining text
            spriteBatch.DrawString(fonts[Game1.BOLD], scoreText + points, scoreLoc, Color.Gold);
            spriteBatch.DrawString(fonts[Game1.BOLD], highText, highLoc, Color.Gold);
            spriteBatch.DrawString(fonts[Game1.BOLD], timeText, timeLoc, Color.Gold);
        }

        //Pre: player is the player
        //Post: none
        //Desc: checks collision in the minigame
        public virtual void HasCollided(Player player) 
        {
            
        }
    }
}
