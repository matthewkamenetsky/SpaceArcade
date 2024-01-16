/* Author: Matthew Kamenetsky
 * File name: RunnerAsteroid.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 28, 2023
 * Modified date: June 08, 2023
 * Description: The RunnerAsteroid class is responsible for updating the runner asteroids in the space run game; it is a child of the SpaceObject class.
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
    class RunnerAsteroid : SpaceObject
    {
        //Store the tolerance of the image for hitbox purposes
        private const int TOLERANCE = 40;

        //Pre: objectImg is the objects image, blankTexture is a blank image, animPos is the animation's position, speed is the object's speed
        //Post: none
        //Desc: sets up the RunnerAsteroid object with necessary values
        public RunnerAsteroid(Texture2D objectImg, Texture2D blankTexture, Vector2 animPos, float speed) : base(objectImg, blankTexture, animPos, speed)
        {
            //Setup the RunnerAsteroid' animation and create its hitbox
            objectAnim = new Animation(objectImg, 8, 8, 64, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 2, animPos, 1f, true);
            hitBox = new Rectangle(objectAnim.destRec.X + TOLERANCE, objectAnim.destRec.Y + TOLERANCE, GetWidth() - TOLERANCE * 2, GetHeight() - TOLERANCE * 2);
        }

        //Pre: gameTime is the number of time passed in the game
        //Post: none
        //Desc: updates the movement of the RunnerAsteroid
        public override void Update(GameTime gameTime)
        {
            //Update the base elements of the space object 
            base.Update(gameTime);

            //Set the runner asteroids' hitbox position
            hitBox.X = objectAnim.destRec.X + TOLERANCE;
        }
    }
}
