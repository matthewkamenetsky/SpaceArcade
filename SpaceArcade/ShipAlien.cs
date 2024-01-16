/* Author: Matthew Kamenetsky
 * File name: ShipAlien.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 12, 2023
 * Modified date: June 03, 2023
 * Description: The ShipAlien class is a child class of the Alien. It is responsible for updating the ship aliens in the game. The ship alien is able to shoot bullets at the player.
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
    class ShipAlien : Alien
    {
        //Store direction indexes
        private const int LEFT = 0;
        private const int DOWN = 1;
        private const int RIGHT = 2;

        //Store the randomizer
        private Random rng = new Random();

        //Store the random direction thresholds booleans
        private int[] threshHolds = new int[3];
        private bool[] hasReached = new bool[3] { false, false, false };

        //Store the ship alien's bullets
        private List<Bullet> bullets = new List<Bullet>();
        private Texture2D bulletImg;

        //Store the ship alien's shooting time, timer, and shoot sound
        private Timer shootTimer;
        private double shootTime = 1500;
        private SoundEffect shootSnd;

        //Pre: shootSnd is the sound the ship alien makes when shooting, screenWidth is the width of the screen, screenHeight is the height of the screen, alienImg is the ship's image, splatImg is the image the ship makes once it dies, bulletImg is the bullet's image, pos is the ship's position, speed is the ship's speed, hp is the ship's hp, points are the points the ship gives when killed, activity is the ship's current activity status
        //Post: none
        //Desc: creates a ship object with necessary starting values
        public ShipAlien(SoundEffect shootSnd, int screenWidth, int screenHeight, Texture2D alienImg, Texture2D splatImg, Texture2D bulletImg, Vector2 pos, float speed, int hp, int points, int activity) : base(alienImg, splatImg, pos, speed, hp, points, activity)
        {
            //Calculate direction thresholds
            threshHolds[LEFT] = rng.Next(0, alienImg.Width * 2 + 1);
            threshHolds[DOWN] = rng.Next(screenHeight / 2 - alienImg.Height * 2, screenHeight / 2 + alienImg.Height * 2 + 1);
            threshHolds[RIGHT] = rng.Next(screenWidth - alienImg.Width * 2, screenWidth - alienImg.Width + 1);

            //Store the bullet image
            this.bulletImg = bulletImg;

            //Setup the shoot sound and timer
            this.shootSnd = shootSnd;
            shootTimer = new Timer(shootTime, true);
        }

        //Pre: gameTime is the current time passed in the game, player is the player, width and height are the screen dimensions
        //Post: returns the bullet the ship alien shot or returns nothing
        //Desc: updates the ship alien, including its movement and its shooting
        public override Bullet Update(GameTime gameTime, Player player, int width, int height)
        {
            //Update the timers
            shootTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            splatTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Checks if the ship alien is not splatting
            if (!splatTimer.IsActive() && activity == ACTIVE)
            {
                //Update the rectangle positions to match the vector position
                alienRec.X = (int)pos.X;
                alienRec.Y = (int)pos.Y;

                //Check which direction has not been reached to determine how the ship alien should move
                if (!hasReached[LEFT])
                {
                    //Decrease the x position by the speed, moving the alien left
                    pos.X -= speed;

                    //Check if the ship alien has reached the threshold
                    if (alienRec.Left <= threshHolds[LEFT])
                    {
                        //Set that the ship alien has reached the left threshold
                        hasReached[LEFT] = true;
                    }
                }
                else if (!hasReached[DOWN])
                {
                    //Increase the y position by the speed, moving the alien down
                    pos.Y += speed;

                    //Check if the ship alien has reached the threshold
                    if (alienRec.Bottom >= threshHolds[DOWN])
                    {
                        //Set that the ship alien has reached the down threshold
                        hasReached[DOWN] = true;
                    }
                }
                else if (!hasReached[RIGHT])
                {
                    //Increase the x position by the speed, moving the alien right
                    pos.X += speed;

                    //Check if the ship alien has reached the threshold
                    if (alienRec.Right >= threshHolds[RIGHT])
                    {
                        //Set that the ship alien has reached the right threshold
                        hasReached[RIGHT] = true;
                    }
                }
                else
                {
                    //Decrease the y position by the speed, moving the alien up
                    pos.Y -= speed;

                    //Check if the alien has passed the bottom of the screen
                    if (alienRec.Bottom <= 0)
                    {
                        //Kill the alien
                        Kill();
                    }
                }

                //Check if the shoot timer is not active
                if (!shootTimer.IsActive()) 
                {
                    //Add a bullet to the list and set its position
                    bullets.Add(new Bullet(bulletImg, pos, 4.75f));
                    bullets.Last().SetPosition(alienRec, AlienShooter.ENEMY);

                    //Play the shooting sound effect
                    SoundEffectInstance shoot = shootSnd.CreateInstance();
                    shoot.Volume = 0.4f;
                    shoot.Play();

                    //Reset the shoot timer
                    shootTimer.ResetTimer(true);

                    //Return the bullet
                    return bullets.Last();
                }
            }
            else if (activity != ACTIVE)
            {
                //Check if the splat timer has finished
                if (splatTimer.IsFinished())
                {
                    //Kill the alien
                    Kill();
                }
            }

            //Return nothing
            return null;
        }
    }
}
