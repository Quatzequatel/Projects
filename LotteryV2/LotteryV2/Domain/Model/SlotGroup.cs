using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Model
{
    public class SlotGroup
    {
        private readonly int _SlotId;
        private readonly Game _Game;
        private Dictionary<SubSets, List<NumberModel>> _Groups = new Dictionary<SubSets, List<NumberModel>>();

        public SlotGroup(int slotId, Game game)
        {
            _SlotId = slotId;
            _Game = game;
            //_Group = group;
        }

        // public void AddNumber(SubSets group, NumberModel value) => _Groups[group].Add(value);

        /// <summary>
        /// Parse values list of numbers into bell curve groups of GroupType.
        /// </summary>
        /// <param name="values"></param>
        public void AddNumbers(IEnumerable<NumberModel> values)
        {
            IEnumerable<NumberModel> slotList = values.Where(i => i.SlotId == _SlotId).ToArray();
            if (slotList.Count() == 0) return;

            int fullGroupSize = slotList.Count();
            int zeroGroupCount = slotList.Where(i => i.PercentChosen == 0).Count();
            int statisticalGroup = zeroGroupCount == 0 ? fullGroupSize : fullGroupSize - zeroGroupCount;
            int subGroupSize = (int)Math.Floor(statisticalGroup / 5.0);
            int stragglers = statisticalGroup % 5;
            foreach (SubSets group in (SubSets[])Enum.GetValues(typeof(SubSets)))
            {
                switch (group)
                {
                    case SubSets.Zero:
                        _Groups[group] = new List<NumberModel>(values.Where(i => i.SlotId == _SlotId && i.PercentChosen == 0).ToArray());
                        break;
                    case SubSets.Low:
                        _Groups[group] = new List<NumberModel>(slotList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize * 4).Take(subGroupSize + stragglers));
                        break;
                    case SubSets.MidLow:
                        _Groups[group] = new List<NumberModel>(slotList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize * 3).Take(subGroupSize));
                        break;
                    case SubSets.Mid:
                        _Groups[group] = new List<NumberModel>(slotList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize * 2).Take(subGroupSize));
                        break;
                    case SubSets.MidHigh:
                        _Groups[group] = new List<NumberModel>(slotList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize).Take(subGroupSize));
                        break;
                    case SubSets.High:
                        _Groups[group] = new List<NumberModel>(slotList.OrderByDescending(i => i.PercentChosen).Take(subGroupSize));
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Quick alias to numbers in a particular group.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<NumberModel> Numbers(SubSets group) => _Groups[group];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public SubSets FindGroupType(int number)
        {
            var results = new List<FindGroup>();
            foreach (SubSets group in (SubSets[])Enum.GetValues(typeof(SubSets)))
            {
                if (_Groups[group].Where(i => i.BallId == number).Count() > 0)
                {
                    string groups = FindGroupTypes(number);
                    SubSets group2 = (SubSets)Enum.Parse(typeof(SubSets), groups);
                    if (group != group2)
                    {
                        Console.WriteLine($"Error in slot template; {number} is wrong {group.ToString()} != {group2.ToString()}");
                    }
                    else return group;
                }
            }

            return default(SubSets);
        }

        /// <summary>
        /// Find all of the groups a number is in.
        /// </summary>
        /// <param name="number">number to look up.</param>
        /// <returns></returns>
        public string FindGroupTypes(int number)
        {
            var items = new List<FindGroup>();

            foreach (SubSets group in (SubSets[])Enum.GetValues(typeof(SubSets)))
            {
                var result = Numbers(group).Where(i => i.BallId == number && i.SlotId == _SlotId)
                    .Select(j => new FindGroup { BallId = j.BallId, Group = group, PercentChosen = j.PercentChosen, TimesChosen = j.TimesChosen }).FirstOrDefault();
                if (result.BallId != 0) items.Add(result);
            }

            //Console.WriteLine($"Id:{number}, items.Count: {items.Count}, {items[0].Group.ToString()}");
            return string.Join("|", items.OrderByDescending(i => i.PercentChosen).Select(i => i.Group).ToArray());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var group in (SubSets[])Enum.GetValues(typeof(SubSets)))
            {
                sb.AppendLine($"{group},list:,{string.Join(",", Numbers(group).Select(i => i.BallId).ToArray())}");
            }
            return sb.ToString();
        }
    }

    public struct FindGroup
    {
        public int BallId;
        public SubSets Group;
        public double PercentChosen;
        public int TimesChosen;
    }
}
