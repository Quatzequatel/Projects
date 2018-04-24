using System;
using System.Collections.Generic;
using System.Linq;
using LotteryV3.Domain.Extensions;

namespace LotteryV3.Domain.Entities
{
    /// <summary>
    /// PropabilityGroup contains
    /// </summary>

    public class PropabilityGroups
    {
        private GameType _game;
        private DrawingContext _Context;
        private Dictionary<int, Dictionary<PropabilityType, PropabilityGroup>> _Groups = new Dictionary<int, Dictionary<PropabilityType, PropabilityGroup>>();

        public PropabilityGroups(DrawingContext context)
        {
            _Context = context;
            _game = _Context.Game;
            for (int slotId = 0; slotId <= context.SlotCount; slotId++)
            {
                Dictionary<PropabilityType, PropabilityGroup> slotGroup = new Dictionary<PropabilityType, PropabilityGroup>();
                foreach (PropabilityType group in (PropabilityType[])Enum.GetValues(typeof(PropabilityType)))
                {
                    slotGroup[group] = new PropabilityGroup(_game, slotId, group);
                    slotGroup[group].AddNumbers(context.GetNumberInfoList());
                }
                _Groups[slotId] = slotGroup;
            }
        }

        public List<PropabilityGroup> Propabilities { get => _Groups.Values.SelectMany(s => s.Values).ToList(); }
        public List<PropabilityGroup> PropabilitiesForSlot(int slotId) => _Groups[slotId].Values.ToList();
        //public List<PropabilityGroup> PropabilitiesForNumber(int number) => Propabilities.Where(i => i.NumberSet().FirstOrDefault(j => j.Id == number) != null).ToList();
        public PropabilityGroup PropabilitiesForNumber(int slotId, int number)
        {
            List<PropabilityGroup> result = new List<PropabilityGroup>();
            foreach (var item in Propabilities.Where(i => i.SlotId == slotId))
            {
                if (item.NumberSet.Where(i => i.Id == number).OrderByDescending(j => j.DrawingsCount).FirstOrDefault() != null) result.Add(item);
            }

            return result.Any() ? result.OrderByDescending(i => i.Propability).FirstOrDefault() : throw new NullReferenceException() ;
        }

        public PropabilityType PropabilityTypeForNumber(int slotId, int number)
        {
            return PropabilitiesForNumber(slotId, number).Propability;
        }
    }
    public class PropabilityGroup
    {
        public readonly int SlotId;
        public readonly GameType Game;
        public readonly PropabilityType Propability;


        private List<NumberInfo> _NumberSet;
        public List<NumberInfo> NumberSet => _NumberSet;

        public string PropabilityDisplay => Propability.ToString();

        public PropabilityGroup(GameType game, int slotId, PropabilityType propability)
        {
            SlotId = slotId;
            Game = game;
            Propability = propability;
        }

        
        public void AddNumbers(List<NumberInfo> values)
        {
           
            List<NumberInfo> slotValuesList = values.Where(i => i.SlotId == SlotId).ToList();
            int fullGroupSize = slotValuesList.Count();
            if (fullGroupSize == 0) return;

            int zeroGroupSize = slotValuesList.Where(i => i.PercentChosen == 0).Count();
            int groupSizeSum = zeroGroupSize == 0 | zeroGroupSize == fullGroupSize ? fullGroupSize : fullGroupSize - zeroGroupSize;

            int subGroupSize = (int)Math.Floor(groupSizeSum / 5.0);
            int remander = groupSizeSum % (subGroupSize * 5);
            int overFlow = (int)Math.Floor(remander / 2.0);

            int skip = groupSizeSum;
            int take = subGroupSize;

            switch (Propability)
            {
                case PropabilityType.Zero:
                    _NumberSet = zeroGroupSize == 0 ? new List<NumberInfo>() : new List<NumberInfo>(values.Where(i => i.SlotId == SlotId && i.PercentChosen <= 0).ToArray());
                    break;
                case PropabilityType.Low:
                    skip -= subGroupSize + overFlow;
                    take += overFlow;
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Skip(skip).Take(take));
                    _NumberSet.ForEach(i => i.GroupType = Propability);
                    break;
                case PropabilityType.MidLow:
                    skip -= 2*(subGroupSize + overFlow);
                    take += overFlow;
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Skip(skip).Take(take));
                    _NumberSet.ForEach(i => i.GroupType = Propability);
                    break;
                case PropabilityType.Mid:
                    skip -= (2 * (subGroupSize + overFlow)) + subGroupSize;
                    take = subGroupSize;
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Skip(skip).Take(take));
                    _NumberSet.ForEach(i => i.GroupType = Propability);
                    break;
                case PropabilityType.MidHigh:
                    skip -= (2 * (subGroupSize + overFlow)) + 2*subGroupSize;
                    take = subGroupSize;
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Skip(skip).Take(take));
                    _NumberSet.ForEach(i => i.GroupType = Propability);
                    break;
                case PropabilityType.High:
                    take = subGroupSize;
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Take(take));
                    _NumberSet.ForEach(i => i.GroupType = Propability);
                    break;
                default:
                    break;
            }

        }

        public string CSVHeading => new string[] { "Game", "Slot", "Propability"}.CSV();
        public string CSVLine => new string[]
        {
            $"{Game}",
            $"{SlotId}",
            $"{Propability.ToString()},"
        }.CSV() + NumberSet.Select(i=> i.Id.ToString()).ToArray().CSV();

    }

}
