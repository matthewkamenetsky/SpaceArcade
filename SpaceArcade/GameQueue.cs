/* Author: Matthew Kamenetsky
 * File name: GameQueue.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 22, 2023
 * Modified date: June 12, 2023
 * Description: implements a minigame queue using standard queue operations
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceArcade
{
    class GameQueue
    {
        //Maintain the queue, and its size
        private Minigame[] queue;
        private int size;

        //Pre: maxSize is the max size of the queue
        //Post: none
        //Desc: creates a minigame queue with necessary values
        public GameQueue(int maxSize)
        {
            //Instantiate the queue to the max size with no elements
            queue = new Minigame[maxSize];
            size = 0;
        }

        //Pre: game is a valid minigame
        //Post: Nnne
        //Description: adds a minigame to the end of the queue if there is room
        public void Enqueue(Minigame game)
        {
            //Check if there is room for the game
            if (size < queue.Length)
            {
                //Add the new item to the end of the queue and increase the queue's size
                queue[size] = game;
                size++;
            }
        }

        //Pre: none
        //Post: returns the front element of the queue
        //Description: returns and removes the element at the front of the queue
        public Minigame Dequeue()
        {
            //Sett the result to null
            Minigame result = null;

            //Check if the queue is not empty
            if (!IsEmpty())
            {
                //Set the result to the front of the queue
                result = queue[0];

                //Loop and move all items one element forward
                for (int i = 1; i < size; i++)
                {
                    //Set the previous item to the current one
                    queue[i - 1] = queue[i];
                }

                //Reduce the size of the queue
                size--;
            }

            //Return the resulting minigame
            return result;
        }

        //Pre: none
        //Post: returns the front element of the queue
        //Description: returns the element at the front of the queue
        public Minigame Peek()
        {
            //Set the result to null
            Minigame result = null;

            //Check if the queue is not empty
            if (!IsEmpty())
            {
                //Set the result to the front of the queue
                result = queue[0];
            }

            //Return the result
            return result;
        }

        //Pre: none
        //Post: returns true if the size of the queue is 0, false otherwise
        //Description: compares the size of the queue against 0 to determine its empty status
        public bool IsEmpty()
        {
            //returns the result of the comparison between size and 0
            return size == 0;
        }
    }
}
