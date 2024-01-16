/* Author: Matthew Kamenetsky
 * File name: SplashAlien.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 13, 2023
 * Modified date: May 26, 2023
 * Description: The SplashAlien class is a child class of the Alien. It is responsible for updating the splash aliens in the game.
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
    class SplashAlien : Alien
    {
        //Store the starting y position and the max y position
        private float startY;
        private int maxY;

        //Pre: alienImg is the splasher's image, splatImg is the image the splasher makes once it dies, pos is the splasher's pos, speed is the splasher's speed, hp is the splasher's hp, points are the points the splasher gives when killed, activity is the splasher's current activity status
        //Post: none
        //Desc: creates a splasher object with necessary starting values
        public SplashAlien(Texture2D alienImg, Texture2D splatImg, Vector2 pos, float speed, int hp, int points, int activity) : base(alienImg, splatImg, pos, speed, hp, points, activity)
        {
            //Setup the start y position and max y position
            startY = pos.Y;
            maxY = (int)startY / 2;
        }

        //Pre: gameTime is the current time passed in the game, player is the player, width and height are the screen dimensions
        //Post: returns nothing
        //Desc: updates the splash alien
        public override Bullet Update(GameTime gameTime, Player player, int width, int height)
        {
            //Update the splat timer
            splatTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Checks if the splasher is not splatting
            if (!splatTimer.IsActive() && activity == ACTIVE)
            {
                //Move the splasher 
                pos.X += speed;
                pos.Y = startY - ((float)(-Math.Pow(pos.X - ((width + alienImg.Width) / 2), 2) * 0.0005) + maxY);
                alienRec.X = (int)pos.X;
                alienRec.Y = (int)pos.Y;

                //Check if the alien has passed the screenwidth
                if (alienRec.Left >= width)
                {
                    //Kill the alien
                    Kill();
                }
            }
            else if (activity != ACTIVE) 
            {
                //Check if the splat timer is finished
                if (splatTimer.IsFinished()) 
                {
                    //Kill the alien
                    Kill();
                }
            }

            //Return nothing
            return null;
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images
        //Post: none
        //Desc: draws the alien or its splat image
        public override void Draw(SpriteBatch spriteBatch)
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
                spriteBatch.Draw(splatImg, splatRec, Color.LightBlue);
            }
        }
    }
}
