using System;
using System.Collections.Generic;


namespace LotteryV2.Domain
{
    public class Number
    {
        private readonly int _Id;
        private readonly int _SlotId;
        private readonly Game _Game;
        public int Id { get => _Id; }
        public int SlotId { get => _SlotId; }
        public Game Game { get => _Game; }

        private readonly List<DateTime> drawingDates = new List<DateTime>();
        public List<DateTime> DrawingDates { get => drawingDates; }
        public void AddDrawingDate(DateTime date) => drawingDates.Add(date);
        public void AddDrawingDates(IEnumerable<DateTime> dates) => drawingDates.AddRange(dates);

        public Number(int id, int slotid, Game game)
        {
            _Id = id;
            _SlotId = slotid;
            _Game = game;
        }
    }
}
