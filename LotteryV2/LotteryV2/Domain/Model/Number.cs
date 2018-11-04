using System;
using System.Collections.Generic;


namespace LotteryV2.Domain.Model
{
    /// <summary>
    /// Simple DTO to store DrawingDates a number was drawn on.
    /// 
    /// </summary>
    public class Number
    {
        public Dictionary<HistoricalPeriods, NumberHistory> HistoricalPerformance { get; set; } = new Dictionary<HistoricalPeriods, NumberHistory>();
        public int BallId { get; private set; }
        public int SlotId { get; private set; }
        public Game Game { get; private set; }

        public List<DateTime> DrawingDates { get; private set; } = new List<DateTime>();
        
        public void AddDrawingDate(DateTime date) => DrawingDates.Add(date);
        public void AddDrawingDates(IEnumerable<DateTime> dates) => DrawingDates.AddRange(dates);

        public Number(int id, int slotid, Game game)
        {
            BallId = id;
            SlotId = slotid;
            Game = game;
        }
    }

    public class NumberHistory
    {
        public HistoricalPeriods Period { get; set; }
        public SubSets Subset { get; set; }
        
        public int DrawCount { get; set; }

        public NumberHistory()
        {

        }

    }
}
