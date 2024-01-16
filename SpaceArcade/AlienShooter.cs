/* Author: Matthew Kamenetsky
 * File name: AlienShooter.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 11, 2023
 * Modified date: June 12, 2023
 * Description: The AlienShooter class is responsible for updating the gameplay of the alien shooter minigame. It manages all of the aliens and how the player interacts with them, as it updates and draws them both.  
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
    class AlienShooter : Minigame
    {
        //Store the player type indexes
        public const int PLAYER = 0;
        public const int ENEMY = 1;

        //Store the alien indexes
        public const int SHOOT_ALIEN = 0;
        public const int SPLASH_ALIEN = 1;
        public const int POISON_ALIEN = 2;

        //Store the time it takes before the current max alien number increases
        private const double INCREASE_RATE = 20000;

        //Store the list of aliens
        private List<Alien> aliens = new List<Alien>();

        //Store the alien images, splat images, hit sounds, and the poison sound
        private Texture2D[] alienImgs;
        private Texture2D[] splatImgs;
        private SoundEffect poisonSnd;

        //Store the alien statistics
        private float[] alienSpeeds = new float[3] { 3.65f, 4.25f, 3.5f };
        private int[] alienHps = new int[3] { 4, 2, 3 };
        private int[] alienPts = new int[3] { 30, 20, 25 };
        private int dmgDealt = 10;

        //Store the list of alien bullets, the bullet images, and the shoot sound
        private List<Bullet> bullets = new List<Bullet>();
        private Texture2D[] bulletImgs;
        private SoundEffect shootSnd;

        //Store the current and maximum amount of aliens
        private int curAliens = 0;
        private int maxAliens = 3;

        //Store the spawn chances
        private int[] spawnChances = new int[3] { 35, 40, 25 };

        //Store the spawn timer and time
        private Timer spawnTimer;
        private double spawnTime = 1200;

        //Store the poison time, and timer
        private double poisonTime = 750;
        private Timer poisonTimer;

        //Pre: fonts are the fonts used in game, bg is the background image, alienImgs are the alien's images, bulletImgs are the bullet images, splatImgs are the images each alien makes when killed, gameSnds are the sounds each alien makes when hit, poison sound is the sound the poison alien makes when it poisons the player, shootSnd is the shooting sound effect, screenWidth and screenHeight are the screen dimensions
        //Post: none
        //Desc: sets up the alien shooter game object with necessary values
        public AlienShooter(SpriteFont[] fonts, Texture2D bg, Texture2D[] alienImgs, Texture2D[] bulletImgs, Texture2D[] splatImgs, SoundEffect[] gameSnds, SoundEffect poisonSnd, SoundEffect shootSnd, int screenWidth, int screenHeight) : base(fonts, bg, gameSnds, screenWidth, screenHeight) 
        {
            //Setup the sound effects
            this.poisonSnd = poisonSnd;
            this.shootSnd = shootSnd;

            //Setup the images
            this.alienImgs = alienImgs;
            this.splatImgs = splatImgs;
            this.bulletImgs = bulletImgs;

            //Setup the timers
            spawnTimer = new Timer(spawnTime, true);
            poisonTimer = new Timer(poisonTime, false);
        }

        //Pre: bullets is the list of bullets, bullet is the bullet being added
        //Post: none
        //Desc: adds an bullet to a list for updating purposes
        private void AddBullet(List<Bullet> bullets, Bullet bullet)
        {
            //Check if the bullet is an bullet
            if (bullet != null)
            {
                //Add the bullet
                bullets.Add(bullet);
            }
        }

        //Pre: gameTime is the number of time passed in the game, player is the player, kb and prevKb are the keyboard input states, mouse and prevMouse are the mouse input states
        //Post: none
        //Desc: updates the alienshooter game by updating timers, spawning aliens, and detecting collision
        public override void UpdateGame(GameTime gameTime, Player player, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse)
        {
            //Update the base elements of the game
            base.UpdateGame(gameTime, player, kb, prevKb, mouse, prevMouse);

            //Update the timers
            spawnTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            poisonTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Check if the time passed is greater than the increase rate and that the max aliens has not been updated yet in order to increase it
            if (gameTimer.GetTimePassed() >= INCREASE_RATE * 2 && maxAliens < 5)
            {
                //Increase the max number of aliens
                maxAliens++;
            }
            else if (gameTimer.GetTimePassed() >= INCREASE_RATE && maxAliens < 4) 
            {
                //Increase the max number of aliens
                maxAliens++;
            }

            //Check if spawn restrictions are lifted
            if (!spawnTimer.IsActive() && curAliens < maxAliens)
            {
                //Spawn the next alien and reset the spawntimer
                SpawnAliens();
                spawnTimer.ResetTimer(true);
            }

            //Check if the player is not being poisoned
            if (!poisonTimer.IsActive())
            {
                //Update the player
                player.Update(gameTime, Game1.SHOOTER, screenWidth, screenHeight, kb, prevKb, mouse, prevMouse);
            }

            //Loop through each bullet to check if one of them hit the player
            for (int i = 0; i < bullets.Count; i++)
            {
                //Update the ship's bullets
                bullets[i].UpdateBullet(ENEMY);

                //Check if the bullet hits the player
                if (player.GetRec().Intersects(bullets[i].GetRec()))
                {
                    //Remove the bullet that hit the player
                    bullets.RemoveAt(i);

                    //Decrease the points
                    points = Math.Max(0, points - dmgDealt);

                    //Play the bullet hit sound effect
                    gameSnds[SHOOT_ALIEN].CreateInstance().Play();
                }
                else if (bullets[i].GetRec().Top >= screenHeight)
                {
                    //Remove the off screen ship bullet
                    bullets.RemoveAt(i);
                }
            }

            //Loop through each player bullet
            for (int i = 0; i < player.GetBullets().Count; i++)
            {
                //Update the player's bullets
                player.GetBullets()[i].UpdateBullet(PLAYER);

                //Check if the player's bullets passed the top of the screen
                if (player.GetBullets()[i].GetRec().Bottom <= 0)
                {
                    //Remove the bullet from the player's list
                    player.GetBullets().RemoveAt(i);
                }
            }

            //Check collision between the player and alien
            HasCollided(player);

            //Loop through the number of alien's for updating purposes
            for (int i = 0; i < aliens.Count; i++)
            {
                //Check that the alien is not dead
                if (aliens[i].GetActivity() != Alien.DEAD)
                {
                    //Update the alien, adding a bullet if it is a ship alien that shot
                    AddBullet(bullets, aliens[i].Update(gameTime, player, screenWidth, screenHeight));
                }
                else
                {
                    //Decrease the number of aliens on screen and remove the alien from the list of aliens
                    curAliens--;
                    aliens.RemoveAt(i);
                }
            }
        }

        //Pre: none
        //Post: none
        //Desc: spawns the next alien by calculating a random spawn chance
        private void SpawnAliens() 
        {
            //Calculate a random spawn chance
            int toSpawn = Game1.rng.Next(1, 101);

            //Store the upcoming alien's position
            Vector2 alienPos;

            //Compare the randomized spawn chance to the spawn chances
            if (toSpawn <= spawnChances[SHOOT_ALIEN])
            {
                //Set the alien's position
                alienPos.X = screenWidth;
                alienPos.Y = Game1.rng.Next(0, alienImgs[SHOOT_ALIEN].Height + 1);

                //Add the alien to the list of aliens
                aliens.Add(new ShipAlien(shootSnd, screenWidth, screenHeight, alienImgs[SHOOT_ALIEN], splatImgs[SHOOT_ALIEN], bulletImgs[ENEMY], alienPos, alienSpeeds[SHOOT_ALIEN], alienHps[SHOOT_ALIEN], alienPts[SHOOT_ALIEN], Alien.ACTIVE));
            }
            else if (toSpawn > spawnChances[SHOOT_ALIEN] && toSpawn <= spawnChances[SHOOT_ALIEN] + spawnChances[SPLASH_ALIEN])
            {
                //Set the alien's position
                alienPos.X = -alienImgs[SPLASH_ALIEN].Width;
                alienPos.Y = Game1.rng.Next(screenHeight / 2 - alienImgs[SPLASH_ALIEN].Height * 2, screenHeight / 2 + alienImgs[SPLASH_ALIEN].Height + 1);

                //Add the alien to the list of aliens
                aliens.Add(new SplashAlien(alienImgs[SPLASH_ALIEN], splatImgs[SPLASH_ALIEN], alienPos, alienSpeeds[SPLASH_ALIEN], alienHps[SPLASH_ALIEN], alienPts[SPLASH_ALIEN], Alien.ACTIVE));
            }
            else 
            {
                //Set the alien's position
                alienPos.X = Game1.rng.Next(0, screenWidth - alienImgs[POISON_ALIEN].Width + 1);
                alienPos.Y = -alienImgs[SHOOT_ALIEN].Height;

                //Add the alien to the list of aliens
                aliens.Add(new PoisonAlien(alienImgs[POISON_ALIEN], splatImgs[POISON_ALIEN], alienPos, alienSpeeds[POISON_ALIEN], alienHps[POISON_ALIEN], alienPts[POISON_ALIEN], Alien.ACTIVE));
            }

            //Increase the number of aliens on screen
            curAliens++;
        }

        //Pre: player is the player
        //Post: none
        //Desc: checks collision between player bullets and aliens, as well as the poison alien and the player
        public override void HasCollided(Player player) 
        {
            //Loop through each alien to check collision
            foreach (Alien alien in aliens)
            {
                //Make sure the alien is active when the collision check is happening
                if (alien.GetActivity() == Alien.ACTIVE)
                {
                    //Check if the alien is a poison alien
                    if (alien is PoisonAlien)
                    {
                        //Check if the player's rectangle intersects the alien's rectangle
                        if (player.GetRec().Intersects(alien.GetRec()))
                        {
                            //Play the poison sound
                            poisonSnd.CreateInstance().Play();

                            //Decrease the points
                            points = Math.Max(0, points - dmgDealt);

                            //Poison the player
                            poisonTimer.ResetTimer(true);

                            //Change the alien's activity
                            alien.ChangeActivity(Alien.POISON);
                        }
                    }

                    //Loop through each of the player's bullets
                    for (int i = 0; i < player.GetBullets().Count; i++)
                    {
                        //Check if the bullet hits the alien
                        if (alien.GetRec().Intersects(player.GetBullets()[i].GetRec()))
                        {
                            //Check which alien was hit to determine what sound effect to play
                            if (alien is ShipAlien)
                            {
                                //Play the shoot alien's hit sound
                                gameSnds[SHOOT_ALIEN].CreateInstance().Play();
                            }
                            else if (alien is SplashAlien)
                            {
                                //Play the splash alien's hit sound
                                gameSnds[SPLASH_ALIEN].CreateInstance().Play();
                            }
                            else
                            {
                                //Play the poison alien's hit sound
                                gameSnds[POISON_ALIEN].CreateInstance().Play();
                            }

                            //Remove the bullet that hit the alien
                            player.GetBullets().RemoveAt(i);

                            //Decrease the alien's hp
                            alien.LowerHp();

                            //Check if the alien has died
                            if (alien.GetHp() == 0)
                            {
                                //Increase the points
                                points += alien.GetPoints();

                                //Activate the splatting state
                                alien.Deactivate();
                            }
                        }
                    }
                }
            }
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images, player is the player
        //Post: none
        //Desc: draws all elements of the alien shooter game, including text, aliens, and the player
        public override void Draw(SpriteBatch spriteBatch, Player player)
        {
            //Draw the background image
            spriteBatch.Draw(bgImg, bgRec, Color.White);

            //Loop through each alien to draw them
            foreach (Alien alien in aliens) 
            {
                //Draw the alien
                alien.Draw(spriteBatch);
            }

            //Draw the player
            player.Draw(spriteBatch, Game1.SHOOTER);

            //Loop through each alien bullet
            foreach (Bullet bullet in bullets) 
            {
                //Draw the bullet
                bullet.DrawBullet(spriteBatch);
            }

            //Loop through each player bullet
            foreach (Bullet bullet in player.GetBullets()) 
            {
                //Draw the bullet
                bullet.DrawBullet(spriteBatch);
            }

            //Draw the base parts of the game
            base.Draw(spriteBatch, player);
        }
    }
}
