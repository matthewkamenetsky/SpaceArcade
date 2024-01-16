/* Author: Matthew Kamenetsky
 * File name: Star.cs
 * Project name: PASS3_ICS4U
 * Creation date: June 04, 2023
 * Modified date: June 08, 2023
 * Description: The Star class is responsible for updating the stars in the star catcher game, including their movement and drawing
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
    class Star
    {
        //Store the speed minimums and maximums
        private const int SPEED_MIN = 3;
        private const int SPEED_MAX = 7;

        //Store the star animation, image, and position
        private Animation starAnim;
        private Texture2D starImg;
        private Vector2 starPos;

        //Store the star's statistic information
        private float[] speeds = new float[2];
        private int[] dirs = new int[2];
        private bool isActive;
        private float diameter;

        //Store the star's change of direction timer and time
        private Timer changeTimer;
        private int changeTime = 2250;

        //Pre: starImg is the star's animation image, starPos is the stars position
        //Post: none
        //Desc: creates a star object with necessary starting values
        public Star(Texture2D starImg, Vector2 starPos) 
        {
            //Store the star's image and position
            this.starImg = starImg;
            this.starPos = starPos;

            //Create the star's animation
            starAnim = new Animation(starImg, 2, 2, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 5, starPos, 1f, true);

            //Create the star's direction change timer
            changeTimer = new Timer(changeTime, true);

            //Randomize the star's direction
            RandomizeDirection();

            //Set the y direction to moving up (visually downwards)
            dirs[StarCatcher.Y] = StarCatcher.UP;

            //Set the star to active
            isActive = true;
        }

        //Pre: none
        //Post: returns the star's rectangle
        //Desc: returns the star's rectangle
        public Rectangle GetRec() 
        {
            //Return the star's rectangle
            return starAnim.destRec;
        }

        //Pre: none
        //Post: returns the star's radius
        //Desc: returns the star's radius
        public float GetRadius() 
        {
            //Returns the star's radius
            return diameter / 2;
        }

        //Pre: none
        //Post: returns the star's acticvity status
        //Desc: returns the star's acticvity status
        public bool IsActive() 
        {
            //returns the star's acticvity status
            return isActive;
        }

        //Pre: type is the coordinate being changed, dir is the direction of the coordinate
        //Post: none
        //Desc: changes the direction of the star 
        public void ChangeDir(int type, int dir)
        {
            //Check if the direction of the coordinate is not the same as the inputted direction
            if (dirs[type] != dir)
            {
                //Set the direction of the coordinate to the inputted one
                dirs[type] = dir;
            }
        }

        //Pre: none
        //Post: none
        //Desc: randomizes the speeds and their directions
        private void RandomizeDirection() 
        {
            //Loop through the speeds
            for (int i = 0; i < speeds.Length; i++)
            {
                //Calculate a new speed
                speeds[i] = Game1.rng.Next(SPEED_MIN, SPEED_MAX + 1);

                //Calculate a temp direction
                int tempDir = Game1.rng.Next(0, 2);

                //Check the value of the temp direction
                if (tempDir == 0)
                {
                    //Set the direction to negative (left if its the x direction, down if its the y direction)
                    dirs[i] = StarCatcher.LEFT;
                }
                else 
                {
                    //Set the direction to positive (right if its the x direction, up if its the y direction)
                    dirs[i] = StarCatcher.RIGHT;
                }
            }

            //Reset the change direction timer
            changeTimer.ResetTimer(true);
        }

        //Pre: toAdd is the vector being added to the star's position
        //Post: none
        //Desc: adds a vector to the star's position to offset collision
        public void AddToPos(Vector2 toAdd)
        {
            //Add the vector to the star's position and update the star's rectangle
            starPos += toAdd;
            starAnim.destRec.X = (int)starPos.X;
            starAnim.destRec.Y = (int)starPos.Y;
        }

        //Pre: none
        //Post: none
        //Desc: deactivates necessary elements of the star
        public void Deactivate() 
        {
            //Move the star offscreen
            starPos.X = -200;
            starAnim.destRec.X = (int)starPos.X;

            //Set the activity of the star to false
            isActive = false;

            //Resets the direction change timer to false
            changeTimer.ResetTimer(false);
        }

        //Pre: startLoc is the starting location of the star
        //Post: none
        //Desc: reactivates the star by resetting its base elements
        public void Reactivate(Vector2 startLoc) 
        {
            //Reset the star's position and its rectangle
            starPos = startLoc;
            starAnim.destRec.X = (int)starPos.X;
            starAnim.destRec.Y = (int)starPos.Y;

            //Randomize the star's direction
            RandomizeDirection();

            //Set the star's y direction
            dirs[StarCatcher.Y] = StarCatcher.UP;

            //Reset the randomization timer
            changeTimer.ResetTimer(true);
            
            //Set the star to active
            isActive = true;
        }

        //Pre: gameTime is the number of time passed in the game
        //Post: none
        //Desc: updates the movement of the star
        public virtual void Update(GameTime gameTime)
        {
            //Check if the star is active
            if (isActive)
            {
                //Update the direction change timer
                changeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                //Update the objects animation
                starAnim.Update(gameTime);

                //Move the star and update its rectangle
                starPos.X += (speeds[StarCatcher.X] * dirs[StarCatcher.X]);
                starPos.Y += (speeds[StarCatcher.Y] * dirs[StarCatcher.Y]);
                starAnim.destRec.X = (int)starPos.X;
                starAnim.destRec.Y = (int)starPos.Y;

                //Check if the change timer stopped
                if (!changeTimer.IsActive())
                {
                    //Randomize the direction and reset the change timer
                    RandomizeDirection();
                }
            }

            //Calculate the diameter of the star
            diameter = Math.Min(starAnim.destRec.Width, starAnim.destRec.Height);
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images
        //Post: none
        //Desc: draws the star
        public void Draw(SpriteBatch spriteBatch)
        {
            //Check if the star is active
            if (IsActive())
            {
                //Draw the star
                starAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            }
        }
    }
}
