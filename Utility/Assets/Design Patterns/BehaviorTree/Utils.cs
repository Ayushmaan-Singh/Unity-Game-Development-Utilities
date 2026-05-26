using System;
using System.Collections.Generic;

namespace Astek.BehaviorTree
{
    public class Utils
    {
        private Random _randomizer = new Random();

        public T[] Shuffle<T>(T[] list)
        {
            int count = list.Length;
            T[] newList = new T[count];
            Array.Copy(list, newList, count);
            while (count > 1)
            {
                int k = _randomizer.Next(count + 1);
                (newList[k], newList[count]) = (newList[count], newList[k]);
            }

            return newList;
        }
    }
}