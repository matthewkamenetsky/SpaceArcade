/* Author: Matthew Kamenetsky
 * File name: SpaceObject.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 26, 2023
 * Modified date: June 08, 2023
 * Description: The SpaceObject class is responsible for updating the space objects in the space run game, and drawing them. It contains necessary accesors and modifiers in order for the objects to be properly included in the space run game.
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
    class SpaceObject
    {
        //Store the object's image, animation, and animation position
        protected Texture objectImg;
        protected Animation objectAnim;
        protected Vector2 animPos;

        //Store the object's blank hitbox texture and rectangle
        protected Texture2D blankTexture;
        protected Rectangle hitBox;

        //Store the object's speed
        protected float speed;

        //Pre: objectImg is the objects image, blankTexture is a blank image, animPos is the animation's position, speed is the object's speed
        //Post: none
        //Desc: sets up the space object with necessary values
        public SpaceObject(Texture2D objectImg, Texture2D blankTexture, Vector2 animPos, float speed) 
        {
            //Setup the object image and position
            this.objectImg = objectImg;
            this.animPos = animPos;

            //Setup its speed
            this.speed = speed;

            //Setup its blank texture for the hitbox
            this.blankTexture = blankTexture;
        }

        //Pre: none
        //Post: returns the object animation's destination rectangle
        //Desc: returns the object animation's destination rectangle for drawing purposes
        public Rectangle GetRec()
        {
            //Return the destination rectangle
            return objectAnim.destRec;
        }

        //Pre: none
        //Post: returns the object's hitbox
        //Desc: returns the object's hitbox for collision purposes
        public Rectangle GetHitBox() 
        {
            //Return the hitbox
            return hitBox;
        }

        //Pre: none
        //Post: returns the object's height
        //Desc: returns the object animation's height for miscellaneous purposes
        public int GetHeight() 
        {
            //Returns the frame height
            return objectAnim.frameHeight;
        }

        //Pre: none
        //Post: returns the object's width
        //Desc: returns the object animation's width for miscellaneous purposes
        public int GetWidth() 
        {
            //Returns the frame width
            return objectAnim.frameHeight;
        }

        //Pre: gameTime is the number of time passed in the game
        //Post: none
        //Desc: updates the movement of the space object
        public virtual void Update(GameTime gameTime)
        {
            //Update the objects animation
            objectAnim.Update(gameTime);

            //Move the object left
            animPos.X -= speed;
            objectAnim.destRec.X = (int)animPos.X;
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images
        //Post: none
        //Desc: draws the space object
        public void Draw(SpriteBatch spriteBatch) 
        {
            //Draw the space object
            objectAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
        }
    }
}
