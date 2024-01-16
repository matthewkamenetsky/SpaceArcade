/* Author: Matthew Kamenetsky
 * File name: CatcherAsteroid.cs
 * Project name: PASS3_ICS4U
 * Creation date: June 05, 2023
 * Modified date: June 08, 2023
 * Description: The CatcherAsteroid class is responsible for updating the asteroids in the star catcher game, including movement and drawing
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
    class CatcherAsteroid
    {
        //Store the tolerance of the image for hitbox purposes
        private const int TOL = 35;

        //Store the asteroid's images, position, animation, and hitbox
        private Texture2D asteroidImg;
        private Texture2D blankTexture;
        private Vector2 animPos;
        private Animation asteroidAnim;
        private Rectangle hitBox;

        //Store the asteroid's statistical information
        private float speed;
        private int dir;

        //Pre: asteroidImg is the asteroid's image, blankTexture is a blank image, animPos is the animation's position, speed is the asteroid's speed, dir is the asteroid's direction
        //Post: none
        //Desc: sets up the CatcherAsteroid with necessary values
        public CatcherAsteroid(Texture2D asteroidImg, Texture2D blankTexture, Vector2 animPos, float speed, int dir)
        {
            //Setup the asteroid's images and position
            this.asteroidImg = asteroidImg;
            this.blankTexture = blankTexture;
            this.animPos = animPos; 

            //Setup the asteroid's statistical information
            this.dir = dir;
            this.speed = speed;

            //Setup the CatcherAsteroid's animation and hitbox
            asteroidAnim = new Animation(asteroidImg, 8, 8, 64, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 2, animPos, 1f, true);
            hitBox = new Rectangle(asteroidAnim.destRec.X + TOL, asteroidAnim.destRec.Y + TOL, asteroidAnim.destRec.Width - TOL * 2, asteroidAnim.destRec.Height - TOL * 2);
        }

        //Pre: none
        //Post: returns the asteroid's width
        //Desc: returns the asteroid's width
        public int GetWidth() 
        {
            //Return the width of the asteroid
            return asteroidAnim.destRec.Width;
        }

        //Pre: none
        //Post: returns the asteroid's height
        //Desc: returns the asteroid's height
        public int GetHeight()
        {
            //Return the height of the asteroid
            return asteroidAnim.destRec.Height;
        }

        //Pre: none
        //Post: returns the asteroid's direction
        //Desc: returns the asteroid's direction
        public int GetDir() 
        {
            //Return the direction of the asteroid
            return dir;
        }

        //Pre: none
        //Post: returns the radius of the hitbox
        //Desc: returns the radius of the hitbox
        public float GetRadius() 
        {
            //Return the hitbox's width divided by 2
            return hitBox.Width / 2;
        }

        //Pre: none
        //Post: returns the hitbox
        //Desc: returns the hitbox
        public Rectangle GetHitBox() 
        {
            //Return the hitbox
            return hitBox;
        }

        //Pre: none
        //Post: returns the asteroid's rectangle
        //Desc: returns the asteroid's rectangle
        public Rectangle GetRec() 
        {
            //Return the asteroid's rectangle
            return asteroidAnim.destRec;
        }

        //Pre: gameTime is the number of time passed in the game
        //Post: none
        //Desc: updates the movement of the asteroid
        public void Update(GameTime gameTime)
        {
            //Update the asteroid's animation
            asteroidAnim.Update(gameTime);

            //Move the asteroid and update its rectangle
            animPos.X += (speed * dir);
            asteroidAnim.destRec.X = (int)animPos.X;

            //Update the hitbox's rectangle
            hitBox.X = asteroidAnim.destRec.X + TOL;
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images
        //Post: none
        //Desc: draws the asteroid
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the asteroid
            asteroidAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
        }
    }
}
