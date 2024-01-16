/* Author: Matthew Kamenetsky
 * File name: Fireball.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 26, 2023
 * Modified date: June 08, 2023
 * Description: The Fireball class is responsible for updating the fireballs in the space run game; it is a child of the SpaceObject class.
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
    class Fireball : SpaceObject
    {
        //Store the width and height tolerance of the image for hit box purposes
        private const int WIDTH_TOL = 42;
        private const int HEIGHT_TOL = 22;

        //Pre: objectImg is the objects image, blankTexture is a blank image, animPos is the animation's position, speed is the object's speed
        //Post: none
        //Desc: sets up the fireball object with necessary values
        public Fireball(Texture2D objectImg, Texture2D blankTexture, Vector2 animPos, float speed) : base(objectImg, blankTexture, animPos, speed)
        {
            //Setup the fireball's animation and create its hitbox
            objectAnim = new Animation(objectImg, 1, 5, 5, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 3, animPos, 1f, true);
            hitBox = new Rectangle(objectAnim.destRec.X + WIDTH_TOL, objectAnim.destRec.Y + HEIGHT_TOL, GetWidth() - WIDTH_TOL * 2, GetHeight() - HEIGHT_TOL * 2);
        }

        //Pre: gameTime is the number of time passed in the game
        //Post: none
        //Desc: updates the movement of the fireball
        public override void Update(GameTime gameTime)
        {
            //Update the animation
            objectAnim.Update(gameTime);

            //Move the animation left
            animPos.X -= (float)(speed * 0.8);
            objectAnim.destRec.X = (int)animPos.X;

            //Move the animation down
            animPos.Y += (float)(speed * 1.15);
            objectAnim.destRec.Y = (int)animPos.Y;

            //Update the hitbox location
            hitBox.X = objectAnim.destRec.X + WIDTH_TOL;
            hitBox.Y = objectAnim.destRec.Y + HEIGHT_TOL;
        }
    }
}
