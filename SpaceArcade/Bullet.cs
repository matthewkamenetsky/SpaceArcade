/* Author: Matthew Kamenetsky
 * File name: Bullet.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 13, 2023
 * Modified date: May 25, 2023
 * Description: The bullet class is responsible for holding onto every single detail about the bullet. This includes position, movement, and more. When called, the bullet's attributes can be accessed, and the bullet can be updated and drawn
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

namespace SpaceArcade
{
    class Bullet
    {
        //Store the bullet's image, position, rectangle, and speed
        private Texture2D bulletImg;
        private Vector2 pos;
        private Rectangle bulletRec;
        private float speed;

        //Pre: bulletImg is the bullet's image, pos is the bullet's position, speed is the bullet's speed
        //Post: none
        //Desc: creates an bullet object with necessary starting values
        public Bullet(Texture2D bulletImg, Vector2 pos, float speed)
        {
            //Set the bullet's image, position, and speed to the inputted ones
            this.bulletImg = bulletImg;
            this.pos = pos;
            this.speed = speed;

            //Create the bullet's rectangle
            bulletRec = new Rectangle((int)pos.X, (int)this.pos.Y, bulletImg.Width, bulletImg.Height);
        }

        //Pre: none
        //Post: return's the bullet's image
        //Desc: return's the bullet's image for miscellaneous reasons
        public Texture2D GetImg()
        {
            //Return the bullet's image
            return bulletImg;
        }

        //Pre: none
        //Post: return's the bullet's rectangle
        //Desc: return's the bullet's rectangle for miscellaneous reasons
        public Rectangle GetRec()
        {
            //Return the bullet's rectangle
            return bulletRec;
        }

        //Pre: rec is the rectangle of the bullet's shooter, user is who the bullet belongs to
        //Post: none
        //Desc: sets the bullet's starting position by calculating x and y values
        public void SetPosition(Rectangle rec, int user)
        {
            //Place the bullet in between the user's rectangle
            bulletRec.X = rec.Center.X - bulletImg.Width / 2;

            //Check the user to determine the y position
            if (user == AlienShooter.PLAYER)
            {
                //Set the bullet's position to the top of the user
                pos.Y = rec.Top - bulletImg.Height;
            }
            else
            {
                //Set the bullet's position to the bottom of the user
                pos.Y = rec.Bottom;
            }

            //Set the new y position of the bullet's rectangle
            bulletRec.Y = (int)pos.Y;
        }

        //Pre: user is who is the shooter of the bullet
        //Post: none
        //Desc: updates the bullet's movement by checking the user to determine directional calculations
        public void UpdateBullet(int user)
        {
            //Check who the user is
            if (user == AlienShooter.PLAYER)
            {
                //Decrease the y position, moving the bullet up
                pos.Y -= speed;
            }
            else
            {
                //Increase the y position, moving the bullet down
                pos.Y += speed;
            }

            //Set the new y position of the bullet's rectangle
            bulletRec.Y = (int)pos.Y;
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images
        //Post: none
        //Desc: draws the bullet
        public void DrawBullet(SpriteBatch spriteBatch)
        {
            //Draws the bullet
            spriteBatch.Draw(bulletImg, bulletRec, Color.White);
        }
    }
}
