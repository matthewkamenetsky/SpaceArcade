/* Author: Matthew Kamenetsky
 * File name: PoisonAlien.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 13, 2023
 * Modified date: May 26, 2023
 * Description: The PoisonAlien class is a child class of the Alien. It is responsible for updating the poison aliens in the game. The poison alien moves towards the player, and if it hits them, they are poisoned and cannot move.
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
    //Pre: alienImg is the poison alien's image, splatImg is the image the poison alien makes once it dies, pos is the poison alien's position, speed is the poison alien's speed, hp is the poison alien's hp, points are the points the poison alien gives when killed, activity is the poison alien's current activity status
    //Post: none
    //Desc: creates a poison alien object with necessary starting values
    class PoisonAlien : Alien
    {
        //Pre: alienImg is the poison alien's image, splatImg is the image the poison alien makes once it dies, pos is the poison alien's position, speed is the poison alien's speed, hp is the poison alien's hp, points are the points the poison alien gives when killed, activity is the poison alien's current activity status
        //Post: none
        //Desc: creates a poison alien object with necessary starting values
        public PoisonAlien(Texture2D alienImg, Texture2D splatImg, Vector2 pos, float speed, int hp, int points, int activity) : base(alienImg, splatImg, pos, speed, hp, points, activity)
        {
        }

        //Pre: gameTime is the current time passed in the game, player is the player, width and height are the screen dimensions
        //Post: returns nothing
        //Desc: updates the poison alien
        public override Bullet Update(GameTime gameTime, Player player, int width, int height)
        {
            //Updates the splat timer
            splatTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Check if the splat timer is not active and if the alien is active or poisoning
            if (!splatTimer.IsActive() && (activity == ACTIVE || activity == POISON))
            {
                //Check the activity to determine movement
                if (activity == ACTIVE)
                {
                    //Calculates the direction vector
                    Vector2 dir = player.GetPos() - pos;

                    //Turns the direction vector towards the player
                    dir.Normalize();

                    //Changes the position of the poison alien and updates its rectangle
                    pos += dir * speed;
                    alienRec.X = (int)pos.X;
                    alienRec.Y = (int)pos.Y;
                }
                else
                {
                    //Increases the y position by the speed and updates the alien's rectangle
                    pos.Y += speed;
                    alienRec.Y = (int)pos.Y;

                    //Check if the alien has passed the bottom of the screen
                    if (alienRec.Top >= height)
                    {
                        //Kill the alien
                        Kill();
                    }
                }
            }
            else if (activity == SPLAT)
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
                spriteBatch.Draw(splatImg, splatRec, Color.GreenYellow);
            }
        }
    }
}
