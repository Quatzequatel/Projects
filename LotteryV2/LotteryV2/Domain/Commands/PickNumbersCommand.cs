using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    class PurmutateNumbers : Command<DrawingContext>
    {

        public override void Execute(DrawingContext context)
        {
            int[] input = new int[] { 1,2,3,4,5,6,7,8,9,10};
            Dictionary<string, LotoNumber> numbers = GetPurmutatedNumbers(input, 6);

            Console.WriteLine($"unique list: {numbers.Count} items");
            foreach (var item in numbers.OrderBy(i => i.Key).ToArray())
            {
                Console.WriteLine(item.Key);
            }
        }

        private Dictionary<string, LotoNumber> GetPurmutatedNumbers(int[] input, int ballCount)
        {
            Dictionary<string, LotoNumber> numbers = new Dictionary<string, LotoNumber>();

            for (int k = 0; k < input.Length; k++)
            {
                int high=input.Length -1;
                int low = k;
                for (int i = input.Length - 1; i != 0; i--)
                {
                    LotoNumber newNum = new LotoNumber();
                    if (newNum.Count != ballCount) newNum.Add(input[k]);
                    if (i != k && newNum.Count < ballCount) newNum.Add(input[i]);

                    while(newNum.Count < ballCount)
                    {
                        high = high == 0 ? input.Length - 1 : --high;
                        if(newNum.Count < ballCount) newNum.Add(input[high]);
                        if (newNum.Count == ballCount) break;
                        low = low >= input.Length-1 ? low % (input.Length-1) : ++low;
                        if (newNum.Count < ballCount) newNum.Add(input[low]);
                    }
                    Console.WriteLine(newNum.ToString());
                    numbers[newNum.ToString()] = newNum;
                }
            }
            return numbers;
        }
    }

    public class LotoNumber
    {
        Dictionary<int, int> numbers = new Dictionary<int, int>();

        public void Add(int value)
        {
            if (numbers.Count <= 6)
            {
                numbers[value] = value;
            }
            else
            {
                throw new IndexOutOfRangeException("number is full.");
            }

        }

        public int Count => numbers.Count;

        public override string ToString()
        {
            return string.Join("-", numbers.OrderBy(i => i.Key).Select(i => i.Key).ToArray());

        }
    }
}
