using System;
using System.Collections.Generic;
using LotteryV2.Domain.Commands;

namespace LotteryV2.Domain
{
    public class HistoricalGroups
    {
        public Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>> PeriodGroups { get; private set; }

        public HistoricalGroups()
        {
            InitializePeriodGroups();
        }

        private void InitializePeriodGroups()
        {
            Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>> periodGroups = new Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>>();

            foreach (var period in (HistoricalPeriods[])Enum.GetValues(typeof(HistoricalPeriods)))
            {
                periodGroups[period] = new Dictionary<int, SlotGroup>();
                for (int slotId = 1; slotId <= new int[0].GetSlotCount(); slotId++)
                {
                    periodGroups[period][slotId] = new SlotGroup(slotId, DrawingContext.GameType);
                }
            }

            PeriodGroups = periodGroups;
        }
    }
}
