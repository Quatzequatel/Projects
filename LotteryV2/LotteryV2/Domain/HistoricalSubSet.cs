using System;
using System.Collections.Generic;


namespace LotteryV2.Domain
{
    /// <summary>
    /// Historical SubSet is the subset value of a number in a period of time.
    /// Ex. HistoricalSubSet(Lotto, 1, 1).PeriodValue(Wk1) retuns Low
    /// </summary>
    public class HistoricalSubSet
    {
        private int _Id;
        private int _SlotId;
        private Game _Game;
        private Dictionary<HistoricalPeriods, SubSets> HistoricalSubSets;

        public HistoricalSubSet(Game game, int slotId, int id)
        {
            _Game = game;
            _SlotId = slotId;
            _Id = id;

            foreach (var period in (HistoricalPeriods[])Enum.GetValues(typeof(HistoricalPeriods)))
            {
                HistoricalSubSets[period] = SubSets.Zero;
            }
        }

        public SubSets PeriodValue(HistoricalPeriods period) => HistoricalSubSets[period];

        /// <summary>
        /// returns an indicator of trend. Is the Id becomming Hot or Cold;
        /// or going from Hot to Warm.
        /// </summary>
        /// <returns>Trending speculation value</returns>
        public object TrendingTowards()
        {
            return null;
        }
    }
}
