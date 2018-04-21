using System;
using System.Collections.Generic;
using System.Linq;
using LotteryV2.Domain.Commands;

namespace LotteryV2.Domain
{
    public static class Groups
    {
        public static Dictionary<int, SlotGroup> DefineGroups(DrawingContext context)
        {
            return DefineGroups(context.SlotCount, context.CurrentGame,
                LoadSlotModel(context.HighestBall, context.SlotCount, context.CurrentGame,
                    context.Drawings.Where(i => i.DrawingDate >= new DateTime(1995, 1, 1)).ToList())
                );
        }
        public static Dictionary<int, SlotGroup> DefineGroups(DrawingContext context, List<NumberModel> numbers)
        {
            return DefineGroups(context.SlotCount, context.CurrentGame, numbers);
        }
        public static Dictionary<int, SlotGroup> DefineGroups(int highestBall, int slotCount, Game game, List<Drawing> drawings)
        {
            return DefineGroups(slotCount, game, LoadSlotModel(highestBall, slotCount, game, drawings));
        }
        public static Dictionary<int, SlotGroup> DefineGroups(int slotCount, Game game, List<NumberModel> numbers)
        {
            Dictionary<int, SlotGroup> groups = new Dictionary<int, SlotGroup>();

            for (int slotid = 0; slotid <= slotCount; slotid++)
            {
                SlotGroup group = new SlotGroup(slotid, game);
                group.AddNumbers(numbers);
                groups[slotid] = group;
            }

            return groups;
        }


        public static List<NumberModel> LoadSlotModel(DrawingContext context)
        {
            return LoadSlotModel(context.HighestBall, context.SlotCount, context.CurrentGame, context.Drawings);
        }
        /// <summary>
        /// Generate model of numbers from list of drawings.
        /// </summary>
        /// <param name="highestBall">highest number ball</param>
        /// <param name="slotCount">number of balls in drawing</param>
        /// <param name="game">type of game</param>
        /// <param name="drawings">pool of drawings to extract from.</param>
        /// <returns></returns>
        public static List<NumberModel> LoadSlotModel(int highestBall, int slotCount, Game game, List<Drawing> drawings)
        {
            List<NumberModel> numbers = new List<NumberModel>();

            //load number model for each slot.
            for (int slot = 1; slot <= slotCount; slot++)
            {
                for (int index = 1; index <= highestBall; index++)
                {
                    var number = new NumberModel(index, slot, game);
                    number.LoadDrawings(drawings);
                    numbers.Add(number);
                }
            }

            //load number model for all slots.
            for (int number = 1; number <= highestBall; number++)
            {
                var element = new NumberModel(number, 0, game);

                foreach (var item in numbers.Where(num => num.Id == number).ToArray())
                {
                    if (element.DrawingsCount == 0)
                    {
                        element.SetDrawingsCount(item.DrawingsCount);
                    }
                    element.AddDrawingDates(item.DrawingDates);
                }
                numbers.Add(element);
            }

            return numbers;
        }
        /// <summary>
        /// generate all permutation of lotter numbers for given set of numbers
        /// </summary>
        /// <param name="input">array of numbers to choose from</param>
        /// <param name="ballCount">number of balls</param>
        /// <returns></returns>
        public static Dictionary<string, LotoNumber> GenerateLotoNumbersFromInputArray(int[] input, int ballCount)
        {
            Dictionary<string, LotoNumber> numbers = new Dictionary<string, LotoNumber>();

            for (int k = 0; k < input.Length; k++)
            {
                int high = input.Length - 1;
                int low = k;
                for (int i = input.Length - 1; i != 0; i--)
                {
                    LotoNumber newNum = new LotoNumber();
                    if (newNum.Count != ballCount) newNum.Add(input[k]);
                    if (i != k && newNum.Count < ballCount) newNum.Add(input[i]);

                    while (newNum.Count < ballCount)
                    {
                        high = high == 0 ? input.Length - 1 : --high;
                        if (newNum.Count < ballCount) newNum.Add(input[high]);
                        if (newNum.Count == ballCount) break;
                        low = low >= input.Length - 1 ? low % (input.Length - 1) : ++low;
                        if (newNum.Count < ballCount) newNum.Add(input[low]);
                    }
                    Console.WriteLine(newNum.ToString());
                    numbers[newNum.ToString()] = newNum;
                }
            }
            return numbers;
        }
    }
}
