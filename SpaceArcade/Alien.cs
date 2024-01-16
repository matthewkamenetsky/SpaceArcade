/* Author: Matthew Kamenetsky
 * File name: Alien.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 11, 2023
 * Modified date: May 26, 2023
 * Description: The Alien class is a parent class to many child aliens. It is responsible for setting up the basic elements of an alien. It contains various accessors and modifiers for aliens. It also has several behaviours that can be overriden by its children, such as updating and drawing the alien.
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
    class Alien
    {
        //Store alien activity indexes
        public const int ACTIVE = 0;
        public const int SPLAT = 1;
        public const int DEAD = 2;
        public const int POISON = 3;

        //Store the alien's sprite related information
        protected Texture2D alienImg;
        protected Rectangle alienRec;
        protected Vector2 pos;

        //Store the alien's statistical information
        protected float speed;
        protected int hp;
        protected int points;

        //Store the alien's activity
        protected int activity;

        //Store the alien's splat image and rectangle
        protected Texture2D splatImg;
        protected Rectangle splatRec;

        //Store the alien's splat timer and splat time
        protected Timer splatTimer;
        private double splatTime = 500;

        //Pre: alienImg is the alien's image, splatImg is the image the alien makes once it dies, pos is the alien's position, speed is the alien's speed, hp is the alien's hp, points are the points the alien gives when killed, activity is the alien's current activity status
        //Post: none
        //Desc: creates a alien object with necessary starting values
        public Alien(Texture2D alienImg, Texture2D splatImg, Vector2 pos, float speed, int hp, int points, int activity)
        {
            //Set the alien's image, position, and create its rectangle
            this.alienImg = alienImg;
            this.pos = pos;
            alienRec = new Rectangle((int)pos.X, (int)pos.Y, alienImg.Width, alienImg.Height);

            //Set the alien's statistical values
            this.speed = speed;
            this.hp = hp;
            this.points = points;

            //Set the alien's activity
            this.activity = activity;

            //Set the alien's splat image and create its rectangle
            this.splatImg = splatImg;
            splatRec = new Rectangle((int)pos.X, (int)pos.Y, splatImg.Width, splatImg.Height);

            //Sets up the splat timer
            splatTimer = new Timer(splatTime, false);
        }

        //Pre: none
        //Post: returns the alien's activity
        //Desc: returns the alien's activity
        public int GetActivity() 
        {
            //returns the alien's activity
            return activity;
        }

        //Pre: none
        //Post: returns the alien's points
        //Desc: returns the alien's points
        public int GetPoints() 
        {
            //returns the alien's points
            return points;
        }

        //Pre: none
        //Post: returns the alien's hp
        //Desc: returns the alien's hp
        public int GetHp() 
        {
            //returns the alien's hp
            return hp;
        }

        //Pre: none
        //Post: returns the alien's rectangle
        //Desc: returns the alien's rectangle
        public Rectangle GetRec() 
        {
            //returns the alien's rectangle
            return alienRec;
        }

        //Pre: none
        //Post: none
        //Desc: lowers the hp of the alien
        public void LowerHp() 
        {
            //Decreases the hp of the alien
            hp--;
        }

        //Pre: type is the activity the alien is being set to
        //Post: none
        //Desc: changes the activity of the alien to the inputted one
        public void ChangeActivity(int type) 
        {
            //Change the activity of the alien
            activity = type;
        }

        //Pre: none
        //Post: none
        //Desc: deactivates the alien, setting it to its splatting phase
        public void Deactivate() 
        {
            //Setup the splatting rectangle
            splatRec = new Rectangle(alienRec.Center.X - splatImg.Width / 2, alienRec.Center.Y - splatImg.Height / 2, splatImg.Width, splatImg.Height);

            //Move the alien image offscreen
            alienRec.X = -200;

            //Set the alien's activity to splatting, and activate the splat timer
            activity = SPLAT;
            splatTimer.Activate();
        }

        //Pre: none
        //Post: none
        //Desc: kills the alien by setting its status to dead
        public void Kill() 
        {
            //Change the alien's activity to dead
            activity = DEAD;
        }

        //Pre: gameTime is the current time passed in the game, player is the player, width and height are the screen dimensions
        //Post: returns nothing or a bullet if the ship alien shot
        //Desc: updates the alien
        public virtual Bullet Update(GameTime gameTime, Player player, int width, int height) 
        {
            //Return nothing
            return null;
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images
        //Post: none
        //Desc: draws the alien or its splat image
        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            //Checks if the alien is alive
            if (!splatTimer.IsActive())
            {
                //Draws the alien
                spriteBatch.Draw(alienImg, alienRec, Color.White);
            }
            else
            {
                //Draws the splat image
                spriteBatch.Draw(splatImg, splatRec, Color.White);
            }
        }
    }
}
