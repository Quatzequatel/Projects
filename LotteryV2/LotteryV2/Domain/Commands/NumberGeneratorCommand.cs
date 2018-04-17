using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    public class NumberGeneratorCommand : Command<DrawingContext>
    {

        public override void Execute(DrawingContext context)
        {
            Dictionary<string, List<int>> unique = GeneratePurmutations(
                new int[] { 13, 11, 2, 7 },
                new int[] { 37, 11, 18 },
                new int[] { 43, 3, 15, 29,19 },
                new int[] { 12, 32, 40, 44 },
                new int[] { 47, 39, 34, 35 },
                new int[] { 8, 20, 45, 46 });
            //13,26,37,19,43,3,12,28,47,39,8

            foreach (var item in unique)
            {
                if (context.Drawings.FirstOrDefault(i => i.KeyString == item.Key) != null)
                    Console.WriteLine(item.Key);
            }
        }

        private Dictionary<string, List<int>> GeneratePurmutations(int[] slot1, int[] slot2, int[] slot3, int[] slot4, int[] slot5, int[] slot6)
        {
            List<List<int>> numbers = new List<List<int>>();
            List<int> input = new List<int>();
            input.AddRange(slot1);
            input.AddRange(slot2);
            input.AddRange(slot3);

            for (int i1 = 0; i1 < slot1.Length; i1++)
            {
                for (int i2 = 0; i2 < slot2.Length; i2++)
                {
                    for (int i3 = 0; i3 < slot3.Length; i3++)
                    {
                        for (int i4 = 0; i4 < slot4.Length; i4++)
                        {
                            for (int i5 = 0; i5 < slot5.Length; i5++)
                            {
                                for (int i6 = 0; i6 < slot6.Length; i6++)
                                {
                                    List<int> number = new List<int> { slot1[i1], slot2[i2], slot3[i3], slot4[i4], slot5[i5], slot6[i6] };
                                    numbers.Add(number.OrderBy(i => i).ToList());
                                }
                            }
                        }
                    }
                }
            }

            Dictionary<string, List<int>> unique = new Dictionary<string, List<int>>();
            foreach (var item in numbers)
            {
                unique[String.Join("-", item.ToArray())] = item;
            }

            return unique;
        }

    }
}
