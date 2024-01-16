/* Author: Matthew Kamenetsky
 * File name: FakeCoin.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 26, 2023
 * Modified date: June 08, 2023
 * Description: The FakeCoin class is responsible for updating the fake coins in the space run game; it is a child of the SpaceObject class.
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
    class FakeCoin : SpaceObject
    {
        //Store the width and height tolerance of the image for hit box purposes
        private const int WIDTH_TOL = 12;
        private const int HEIGHT_TOL = 2;

        //Pre: objectImg is the objects image, blankTexture is a blank image, animPos is the animation's position, speed is the object's speed
        //Post: none
        //Desc: sets up the FakeCoin object with necessary values
        public FakeCoin(Texture2D objectImg, Texture2D blankTexture, Vector2 animPos, float speed) : base(objectImg, blankTexture, animPos, speed)
        {
            //Setup the FakeCoin's animation and create its hitbox
            objectAnim = new Animation(objectImg, 8, 1, 8, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 3, animPos, 1f, true);
            hitBox = new Rectangle(objectAnim.destRec.Center.X - hitBox.Width / 2, objectAnim.destRec.Y + HEIGHT_TOL, GetWidth() - WIDTH_TOL * 2, GetHeight() - HEIGHT_TOL * 2);
        }

        //Pre: gameTime is the number of time passed in the game
        //Post: none
        //Desc: updates the movement of the Fake Coin
        public override void Update(GameTime gameTime)
        {
            //Update the base elements of the space object
            base.Update(gameTime);

            //Set the fake coin's hitbox position
            hitBox.X = objectAnim.destRec.Center.X - hitBox.Width / 2;
            hitBox.Y = objectAnim.destRec.Y + HEIGHT_TOL;
        }
    }
}
