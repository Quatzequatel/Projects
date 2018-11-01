using System;
using System.Collections.Generic;
using System.Linq;
using LotteryV2.Domain.Commands;

namespace LotteryV2.Domain
{
    /// <summary>
    /// The purpose of the model is to reduce the choosen set to something where all purmuations
    /// of the set can reasonably purchased and the set has a better than average probablity of 
    /// containing the winning numbers. 
    /// 
    /// Groups is a model for lottery numbers. it is based on a premise that:
    /// 1. truely random numbers are generally equally chosen in time.
    /// 2. lottery numbers are sorted in asending order. 
    /// 3. only one number can be chosen per ball/slot per drawing. 
    ///     Exception is PowerBall or MegaBall these are additional set of numbers.
    /// 4. due to sorting the 1 ball must always be in the first slot and 
    ///     the highest ball must always be in the last slot.
    /// 5. The lottery is choosen randomly so i have created 
    ///     a concept of patterns to help choose numbers. 
    /// Lottery games are derived from a time when numbered balls 
    ///     were choosen by spinning a container and randomly choosing a ball 
    ///     then placing the ball into a display slot until all balls were chosen.
    ///     Order did not matter for simplification balls were displayed in ascending 
    ///     order.
    ///  Slot is a representation of the display.
    ///  1. each slot contains a dictionary of numbers/numberModel; the key is the ball number.
    ///  2. Number is a object that contains the ball number, slot index and dates number was drawn.
    ///  3. NumberModel inherits Number as a wrapper for Statistical information; primarily 
    ///     TimesChosen - The number of times number was choosen in slot. => DrawingDates.Count
    ///     PercentChosen - TimesChosen/Number of drawings for given dataset.
    ///     [Sum] - is the sum of all balls choosen for a drawing.
    ///     TotalSum - The sum of sums.
    ///     AvgSum - The average sum of drawings.
    ///     Min/MaxSum - The min or max sum of a drawing.
    ///  4. Slot[0] represents summary of all other slots; except Power/Mega Ball.
    ///     Ex. Slot[0][id].TimesChosen == Slot[1][id].TimesChosen + Slot[2][id].TimesChosen .. + Slot[last][id].TimesChosen
    ///  
    /// Sub-Sets:
    /// Sub-sets is a template concept. Where the number set is divided into sub-sets.
    /// i have decided to use % choosen to create my sub-sets. There are 6 sub-sets;
    /// [Zero, low, midLow, Mid, MidHigh, High] sets. the size of each sub-set can 
    /// vary from slot to slot and among groups. Ex Zero set can be almost 1/2 the 
    /// numbers for some slots.
    /// 
    /// Zero set contains all numbers never choosen for given slot.
    /// 
    /// After Zero set is defined the rest the set are ([all numbers] - [zero set])/5 in size.
    /// Any remainder is split amont low and midlow groups. the idea being the high + midhigh 
    /// groups will account for 60% of the selected numbers.
    /// 
    /// low  
    /// midLow, 
    /// Mid, 
    /// MidHigh, 
    /// High
    /// 
    /// Assumptions -The general goal is to smartly filter numbers out.
    /// 1. numbers that have never been choosen for a slot are not like to ever 
    /// be choosen for a given slot. Don't not polute a given slot with such numbers.
    ///     Ex. 1 can only be in slot[1], highest ball can only be in slot[last].
    ///     Ex. 2 can only be in slot[1] or slot[2], 2nd highest can only be in slot[last-1] or slot[last].
    /// 2. choosen items tend to follow a bell curve, such that a small % 
    /// of numbers are choosen most of the time for a given slot[]. With that in mind sub-set numbers into
    /// [Zero, low, midLow, Mid, MidHigh, High] sets.
    /// 
    /// Template for a 5 slot game is something like this;
    /// 1[high], 2[high], 3[midhigh], 4[mid], 5[high].
    /// Each drawing has a template pattern. There are 5! (120) templates for a 5 slot game.
    /// Assumption is Zero sub-set is never apart of a template
    /// Following the same logic as the that certain templates have a higher probabilty than others;
    /// Templates are defined as [Blue, Aqua, Sunrise, RedHot]
    /// RedHot - the top 5 templates.
    /// Sunrise - top [6 - 20] templates
    /// Aqua - top [21-45] templates
    /// Blue - the remaining templates.
    ///  
    /// </summary>
    public static class Groups
    {

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
            return LoadSlotModel(context.HighestBall, context.SlotCount, DrawingContext.GameType, context.Drawings);
        }
        /// <summary>
        /// Generate model of numbers from list of drawings.
        /// </summary>
        /// <param name="highestBall">highest number ball</param>
        /// <param name="slotCount">number of balls in drawing</param>
        /// <param name="game">type of game</param>
        /// <param name="drawings">pool of drawings to extract from.</param>
        /// <returns></returns>
        private static List<NumberModel> LoadSlotModel(int highestBall, int slotCount, Game game, List<Drawing> drawings)
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

                foreach (var item in numbers.Where(num => num.BallId == number).ToArray())
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
