using System;
using System.Collections.Generic;


namespace LotteryV2.Domain
{
    /// <summary>
    /// Simple DTO to store DrawingDates a number was drawn on.
    /// 
    /// </summary>
    public class Number
    {
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
}
