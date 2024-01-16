/* Author: Matthew Kamenetsky
 * File name: SpaceObjectQueue.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 28, 2023
 * Modified date: June 05, 2023
 * Description: the SpaceObjectQueue class is an implementation of an item queue, using space objects
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceArcade
{
    class SpaceObjectQueue
    {
        //Store the size of the queue and the queue
        private int size = 0;
        private SpaceObject[] queue;

        //Pre: none 
        //Post: none
        //Desc: initializes the queue 
        public SpaceObjectQueue(int maxSize)
        {
            //Setup the queue
            queue = new SpaceObject[maxSize];
        }

        //Pre: spaceObject is the space object to add
        //Post: none
        //Desc: adds a space object to the queue of space objects
        public void Enqueue(SpaceObject spaceObject)
        {
            //Check if the size of the queue is less than the queue length
            if (Size() < queue.Length)
            {
                //Add the spaceobject to the queue and increase the size
                queue[size] = spaceObject;
                size++;
            }
        }

        //Pre: none
        //Post: return the dequeued object
        //Desc: dequeues a space object from the list and returns it
        public SpaceObject Dequeue()
        {
            //Check if the queue is not empty
            if (!IsEmpty())
            {
                //Store the temp space object
                SpaceObject temp = queue[0];

                //Loop through the size of the queue minus 1
                for (int i = 0; i < size - 1; i++)
                {
                    //Set the queue at the current number to the number plus 1
                    queue[i] = queue[i + 1];
                }

                //Decrease the size
                size--;

                //Return the temp space object
                return temp;
            }

            //return null
            return null;
        }

        //Pre: none
        //Post: returns the size of the queue
        //Desc: returns the size of the queue
        public int Size()
        {
            //Return the size
            return size;
        }

        //Pre: none
        //Post: returns if the queue is empty
        //Desc: returns if the queue is empty
        public bool IsEmpty() 
        {
            //Return if the queue is empty
            return queue.Length == 0;
        }
    }
}
