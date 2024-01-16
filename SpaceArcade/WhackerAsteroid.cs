/* Author: Matthew Kamenetsky
 * File name: WhackerAsteroid.cs
 * Project name: PASS3_ICS4U
 * Creation date: June 08, 2023
 * Modified date: June 12, 2023
 * Description: The WhackerAsteroid class is responsible for updating the asteroids in the asteroid whacker game
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
    class WhackerAsteroid
    {
        //Store the activity indexes
        public const int POOF = 0;
        public const int EXPLODE = 1;
        public const int ACTIVE = 2;
        public const int INACTIVE = 3;

        //Store the asteroid's images, position, rectangles, and animations
        private Texture2D[] asteroidImgs = new Texture2D[3];
        private Vector2 pos;
        private Rectangle[] asteroidRecs = new Rectangle[2];
        private Animation[] asteroidAnims = new Animation[2];

        //Store the asteroid's statistical information
        private int isActive = INACTIVE;

        //Pre: asteroidImg is the asteroid's image, pos is the asteroid's position
        //Post: none
        //Desc: sets up the WhackerAsteroid with necessary values
        public WhackerAsteroid(Texture2D[] asteroidImgs, Vector2 pos)
        {
            //Setup the asteroid's images and position
            this.asteroidImgs = asteroidImgs;
            this.pos = pos;

            //Setup the asteroids rectangles and animations
            asteroidRecs[WhackAnAsteroid.CIRCLE] = new Rectangle((int)pos.X, (int)pos.Y, asteroidImgs[WhackAnAsteroid.CIRCLE].Width, asteroidImgs[WhackAnAsteroid.CIRCLE].Height);
            asteroidRecs[WhackAnAsteroid.ASTEROID] = new Rectangle(asteroidRecs[WhackAnAsteroid.CIRCLE].Center.X - asteroidImgs[WhackAnAsteroid.ASTEROID].Width / 2, asteroidRecs[WhackAnAsteroid.CIRCLE].Center.Y - asteroidImgs[WhackAnAsteroid.ASTEROID].Height / 2, asteroidImgs[WhackAnAsteroid.ASTEROID].Width, asteroidImgs[WhackAnAsteroid.ASTEROID].Height);
            asteroidAnims[POOF] = new Animation(asteroidImgs[WhackAnAsteroid.POOF], 5, 1, 5, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 3, pos, 1f, false);
            asteroidAnims[EXPLODE] = new Animation(asteroidImgs[WhackAnAsteroid.EXPLOSION], 3, 3, 9, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 2, pos, 1f, false);
        }

        //Pre: none
        //Post: returns the activity of the asteroid
        //Desc: returns the activity of the asteroid
        public int GetActivity() 
        {
            //Return the activity of the asteroid
            return isActive;
        }

        //Pre: none
        //Post: returns the asteroid's rectangle
        //Desc: returns the asteroid's rectangle
        public Rectangle GetRec()
        {
            //Return the asteroid's rectangle
            return asteroidRecs[WhackAnAsteroid.ASTEROID];
        }

        //Pre: none
        //Post: none
        //Desc: activates the asteroid
        public void Activate() 
        {
            //Set the activity to active
            isActive = ACTIVE;
        }

        //Pre: none
        //Post: none
        //Desc: poofs the asteroid
        public void Kill() 
        {
            //Setup the animation and set the activity to poof
            asteroidAnims[POOF].destRec.X = asteroidRecs[WhackAnAsteroid.ASTEROID].Center.X - asteroidAnims[POOF].destRec.Width / 2;
            asteroidAnims[POOF].destRec.Y = asteroidRecs[WhackAnAsteroid.ASTEROID].Center.Y - asteroidAnims[POOF].destRec.Height / 2;
            asteroidAnims[POOF].isAnimating = true;
            isActive = POOF;
        }

        //Pre: none
        //Post: none
        //Desc: explodes the asteroid
        public void Explode() 
        {
            //Setup the animation and set the activity to explode
            asteroidAnims[EXPLODE].destRec.X = asteroidRecs[WhackAnAsteroid.ASTEROID].Center.X - asteroidAnims[EXPLODE].destRec.Width / 2;
            asteroidAnims[EXPLODE].destRec.Y = asteroidRecs[WhackAnAsteroid.ASTEROID].Center.Y - asteroidAnims[EXPLODE].destRec.Height / 2;
            asteroidAnims[EXPLODE].isAnimating = true;
            isActive = EXPLODE;
        }

        //Pre: gameTime is the number of time passed in the game
        //Post: none
        //Desc: updates the asteroid's animations
        public void Update(GameTime gameTime)
        {
            //Check if the activity is less than the active one
            if (isActive < ACTIVE)
            {
                //Loop through the animations
                for (int i = 0; i < asteroidAnims.Length; i++)
                {
                    //Check if the current activity is the same as the current index
                    if (isActive == i)
                    {
                        //Update the animation at the current index
                        asteroidAnims[i].Update(gameTime);

                        //Check if the animation is done
                        if (asteroidAnims[i].isAnimating == false)
                        {
                            //Set the asteroid to inactive
                            isActive = INACTIVE;
                        }
                    }
                }
            }
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images
        //Post: none
        //Desc: draws the asteroid
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draws the circle around the asteroid
            spriteBatch.Draw(asteroidImgs[WhackAnAsteroid.CIRCLE], asteroidRecs[WhackAnAsteroid.CIRCLE], Color.White);

            //Checks if the asteroid is active
            if (isActive == ACTIVE)
            {
                //Draw the asteroid
                spriteBatch.Draw(asteroidImgs[WhackAnAsteroid.ASTEROID], asteroidRecs[WhackAnAsteroid.ASTEROID], Color.White);
            }

            //Loops through the activities less than the active one
            for (int i = 0; i < ACTIVE; i++) 
            {
                //Check if the activity is the current one
                if (isActive == i) 
                {
                    //Draw the animation
                    asteroidAnims[i].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
                }
            }
        }
    }
}
