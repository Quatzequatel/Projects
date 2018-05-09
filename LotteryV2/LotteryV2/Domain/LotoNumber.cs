using System;
using System.Collections.Generic;
using System.Linq;

namespace LotteryV2.Domain
{
    public class LotoNumber
    {
        Dictionary<int, int> numbers = new Dictionary<int, int>();

        public void Add(int value)
        {
            if (numbers.Count < new int[0].GetSlotCount())
            {
                numbers[value] = value;
            }
            else
            {
                throw new IndexOutOfRangeException("number is full.");
            }

        }

        public int Count => numbers.Count;

        public int[] Numbers => numbers.OrderBy(i => i.Key).Select(i => i.Key).ToArray();

        public int Sum => Numbers.Sum();

        public override string ToString()
        {
            return string.Join("-", numbers.OrderBy(i => i.Key).Select(i => i.Key).ToArray());

        }
    }
}
