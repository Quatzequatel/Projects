using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain
{
    public class SlotGroup
    {
        private readonly int _SlotId;
        private readonly Game _Game;
        private Dictionary<GroupType, List<NumberModel>> _Groups = new Dictionary<GroupType, List<NumberModel>>();

        public SlotGroup(int slotId, Game game)
        {
            _SlotId = slotId;
            _Game = game;
            //_Group = group;
        }

        public void AddNumber(GroupType group, NumberModel value) => _Groups[group].Add(value);

        /// <summary>
        /// Parse values list of numbers into bell curve groups of GroupType.
        /// </summary>
        /// <param name="values"></param>
        public void AddNumbers(IEnumerable<NumberModel> values)
        {
            IEnumerable<NumberModel> slotList = values.Where(i => i.SlotId == _SlotId).ToArray();
            if (slotList.Count() == 0) return;

            int fullGroupSize = slotList.Count();
            int zeroGroupCount = slotList.Where(i => i.PercentChosen <= 0.1).Count();
            int groupSize = zeroGroupCount == 0 ? fullGroupSize : fullGroupSize - zeroGroupCount;
            int subGroupSize = (int)Math.Floor(groupSize / 5.0);
            foreach (GroupType group in (GroupType[])Enum.GetValues(typeof(GroupType)))
            {
                switch (group)
                {
                    case GroupType.Zero:
                        _Groups[group] = new List<NumberModel>(values.Where(i => i.SlotId == _SlotId && i.PercentChosen <= 0.1).ToArray());
                        break;
                    case GroupType.Low:
                        _Groups[group] = new List<NumberModel>(slotList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize * 4).Take(subGroupSize));
                        break;
                    case GroupType.MidLow:
                        _Groups[group] = new List<NumberModel>(slotList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize * 3).Take(subGroupSize));
                        break;
                    case GroupType.Mid:
                        _Groups[group] = new List<NumberModel>(slotList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize * 2).Take(subGroupSize));
                        break;
                    case GroupType.MidHigh:
                        _Groups[group] = new List<NumberModel>(slotList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize).Take(subGroupSize));
                        break;
                    case GroupType.High:
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
        public List<NumberModel> Numbers(GroupType group) => _Groups[group];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public GroupType FindGroupType(int number)
        {
            var results = new List<FindGroup>();
            foreach (GroupType group in (GroupType[])Enum.GetValues(typeof(GroupType)))
            {
                if (_Groups[group].Where(i => i.Id == number).Count() > 0) return group;
            }

            return default(GroupType);
        }

        /// <summary>
        /// Find all of the groups a number is in.
        /// </summary>
        /// <param name="number">number to look up.</param>
        /// <returns></returns>
        public string FindGroupTypes(int number)
        {
            var items = new List<FindGroup>();

            foreach (GroupType group in (GroupType[])Enum.GetValues(typeof(GroupType)))
            {
                var result =Numbers(group).Where(i => i.Id == number && i.SlotId == _SlotId)
                    .Select(j => new FindGroup { Id = j.Id, Group = group, PercentChosen = j.PercentChosen, TimesChosen = j.TimesChosen }).FirstOrDefault();
                if (result.Id != 0) items.Add(result);
            }

            return string.Join("|", items.OrderByDescending(i => i.PercentChosen).Select(i=> i.Group).ToArray());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var group in (GroupType[])Enum.GetValues(typeof(GroupType)))
            {
                sb.AppendLine($"{group},list:,{string.Join(",", Numbers(group).Select(i => i.Id).ToArray())}");
            }
            return sb.ToString();
        }
    }

    public struct FindGroup
    {
        public int Id;
        public GroupType Group;
        public double PercentChosen;
        public int TimesChosen;
    }
}
