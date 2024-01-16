/* Author: Matthew Kamenetsky
 * File name: Selector.cs
 * Project name: PASS3_ICS4U
 * Creation date: June 04, 2023
 * Modified date: June 07, 2023
 * Description: The Selector class is responsible for updating and drawing the selector used for choosing the speed and direction of the net in the star catchers game.  
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
    class Selector
    {
        //Store the selector dimension data
        private const int SELECTOR_WIDTH = 128;
        private const int SELECTOR_HEIGHT = 64;

        //Store the speed divider
        private const int SPEED_DIVIDER = 8;

        //Store the indexes of selector images
        public const int BLANK = 0;
        public const int BTN = 1;

        //Store the selecter image indexes
        private const int MIDDLE = 0;
        private const int LEFT = 1;
        private const int RIGHT = 2;
        private const int MOVER = 3;

        //Store the selector button font
        private SpriteFont boldFont;

        //Store the selector images, rectangles, and the position of the moving selector
        private Texture2D[] selectorImgs = new Texture2D[2];
        private Rectangle[] selectorRecs = new Rectangle[4];
        private Rectangle btnRec;
        private Vector2 moverPos;

        //Store the selector button text and its location
        private string stopText = "STOP";
        private Vector2 stopLoc;

        //Store the direction of the mover
        private bool isMovingRight = true;

        //Store whether or not the user has clicked the button
        private bool hasClicked = false;

        //Store the selector speed and the calculated x speed and direction from pressing the button
        private float selectorSpeed = 4f;
        private float xSpeed;
        private int dir;

        //Pre: boldFont is the font used for the button text, selectorImgs are the selector images, screenWidth and screenHeight are the screen dimensions
        //Post: none
        //Desc: creates a selector object using necessary values
        public Selector(SpriteFont boldFont, Texture2D[] selectorImgs, int screenWidth, int screenHeight)
        {
            //Store the font
            this.boldFont = boldFont;

            //Store the selector images
            this.selectorImgs = selectorImgs;

            //Create the selector rectangles
            selectorRecs[MIDDLE] = new Rectangle((screenWidth - SELECTOR_WIDTH) / 2, screenHeight - (int)(SELECTOR_HEIGHT * 2.5), SELECTOR_WIDTH, SELECTOR_HEIGHT / 6);
            selectorRecs[LEFT] = new Rectangle(selectorRecs[MIDDLE].Left - SELECTOR_WIDTH / 10, selectorRecs[MIDDLE].Center.Y - SELECTOR_HEIGHT / 2, SELECTOR_WIDTH / 10, SELECTOR_HEIGHT);
            selectorRecs[RIGHT] = new Rectangle(selectorRecs[MIDDLE].Right, selectorRecs[LEFT].Y, selectorRecs[LEFT].Width, selectorRecs[LEFT].Height);
            selectorRecs[MOVER] = new Rectangle(selectorRecs[LEFT].X, selectorRecs[LEFT].Y, selectorRecs[LEFT].Width / 2, selectorRecs[LEFT].Height);
            
            //Create the mover's position
            moverPos = new Vector2(selectorRecs[MOVER].X, selectorRecs[MOVER].Y);
            
            //Create the button rectangle and its text position
            btnRec = new Rectangle(selectorRecs[MIDDLE].Center.X - selectorImgs[BTN].Width / 2, selectorRecs[LEFT].Bottom + Game1.TITLE_OFFSET / 4, selectorImgs[BTN].Width, selectorImgs[BTN].Height);
            stopLoc = new Vector2(btnRec.Center.X - boldFont.MeasureString(stopText).X / 2 + 1, btnRec.Center.Y - boldFont.MeasureString(stopText).Y / 2);
        }

        //Pre: none
        //Post: returns the calculated x speed
        //Desc: returns the calculated x speed
        public float GetSpeed() 
        {
            //Return the x speed
            return xSpeed;
        }

        //Pre: none
        //Post: returns the calculated direction
        //Desc: returns the calculated direction
        public int GetDir()
        {
            //Return the direction
            return dir;
        }

        //Pre: none
        //Post: returns the click status of the button
        //Desc: returns the click status of the button
        public bool HasClicked() 
        {
            //Return the click status
            return hasClicked;
        }

        //Pre: none
        //Post: returns the y location of the selector
        //Desc: returns the highest position of the selector
        public int GetTop() 
        {
            //Return the y position of the left selector rectangle
            return selectorRecs[LEFT].Y;
        }

        //Pre: none
        //Post: none
        //Desc: resets the click status of the button
        public void ResetClick()
        {
            //Set the click status to false
            hasClicked = false;
        }

        //Pre: mouse is the current state of the mouse, prevMouse is the previous state of the mouse
        //Post: none
        //Desc: updates the selector by moving the mover piece until the button is clicked
        public void Update(MouseState mouse, MouseState prevMouse)
        {
            //Check if the player has not yet clicked the button
            if (!hasClicked)
            {
                //Check the direction the mover is going in
                if (isMovingRight)
                {
                    //Move the mover right
                    moverPos.X += selectorSpeed;

                    //Check if the mover rec has reached the right limit
                    if (selectorRecs[MOVER].Right >= selectorRecs[RIGHT].Right)
                    {
                        //Change the direction
                        isMovingRight = false;
                    }
                }
                else
                {
                    //Move the mover left
                    moverPos.X -= selectorSpeed;

                    //Check if the mover rec has reached the left limit
                    if (selectorRecs[MOVER].Left <= selectorRecs[LEFT].Left)
                    {
                        //Change the direction
                        isMovingRight = true;
                    }
                }

                //Update the mover rectangle x position
                selectorRecs[MOVER].X = (int)moverPos.X;

                //Check player input to determine how to proceed
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    //Check if the button rectangle contains the mouse
                    if (btnRec.Contains(mouse.Position))
                    {
                        //Check if the mover is on the left or right side of the middle of the selector
                        if (selectorRecs[MOVER].Left >= selectorRecs[MIDDLE].Center.X)
                        {
                            //Calculate the xSpeed and set the direction
                            xSpeed = selectorRecs[MOVER].X - selectorRecs[MIDDLE].Center.X;
                            dir = StarCatcher.RIGHT;
                        }
                        else 
                        {
                            //Calculate the xSpeed and set the direction
                            xSpeed = Math.Abs(selectorRecs[MOVER].Right - selectorRecs[MIDDLE].Center.X);
                            dir = StarCatcher.LEFT;
                        }

                        //Divide the xSpeed to make it slower
                        xSpeed = xSpeed / SPEED_DIVIDER;

                        //Set that the player has clicked the button
                        hasClicked = true;
                    }
                }
            }
        }

        //Pre: spriteBatch is a SpriteBatch variable that is used for drawing images
        //Post: none
        //Desc: draws all elements of the selector
        public void Draw(SpriteBatch spriteBatch) 
        {
            //Check if the user has not clicked
            if (!hasClicked)
            {
                //Loop through the elements of the selector to draw them
                for (int i = 0; i < MOVER; i++)
                {
                    //Draw the selector
                    spriteBatch.Draw(selectorImgs[BLANK], selectorRecs[i], Color.White);
                }

                //Draw the moving selector
                spriteBatch.Draw(selectorImgs[BLANK], selectorRecs[MOVER], Color.Red);

                //Draw the button
                spriteBatch.Draw(selectorImgs[BTN], btnRec, Color.White);

                //Draw the button text
                spriteBatch.DrawString(boldFont, stopText, stopLoc, Color.White);
            }
        }
    }
}
