using System;
using System.Collections.Generic;

namespace LotteryV2
{
    public class Number
    {
        private readonly int _Id;
        private readonly int _SlotId;
        public int Id { get => _Id; }
        public int SlotId { get => _SlotId; }
        private readonly List<DateTime> drawingDates = new List<DateTime>();
        public List<DateTime> DrawingDates { get => drawingDates; }
        public void AddDrawingDate(DateTime date) => drawingDates.Add(date);
        public Number(int id, int slotid)
        {
            _Id = id;
            _SlotId = slotid;
        }
    }
}
