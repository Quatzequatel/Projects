using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LotteryV3.Domain.Entities
{
    /// <summary>
    /// Number stores basic information about a ball; such as all dates drawn for given slot
    /// </summary>
    public class Number
    {
        private int _Id;
        private int _slotId;
        private GameType _game;

        public readonly DateTime FirstDrawingDate;

        private  List<DateTime> drawingDates = new List<DateTime>();
        [JsonIgnore]
        public List<DateTime> DrawingDates { get => drawingDates; }

        private List<int> _DaysSincePreviousDrawing = new List<int>();
        

        [JsonIgnore]
        public List<int> DrawingIntervals { get => _DaysSincePreviousDrawing.ToList(); }

        public int Id { get => _Id; }
        public int SlotId { get => _slotId; }
        public GameType Game { get => _game; }

        public void AddDrawingDate(int slotId, int number, DateTime date)
        {
            AddDrawingIntervalDate(slotId, number, date);
            drawingDates.Add(date);
        }
        public void AddDrawingIntervalDate(int slotId, int number, DateTime date)
        {
            int interval = drawingDates.Count > 0 ? date.Subtract(drawingDates.Last()).Days : date.Subtract(FirstDrawingDate).Days;
            //TrendDictionary[date] = new TrendValue(slotId, number, interval, date);
            _DaysSincePreviousDrawing.Add(interval);
        }

        public int GetIntervalForGivenDate(DateTime date)
        {
            for (int index = 0; index < drawingDates.Count; index++)
            {
                if (drawingDates[index] == date)
                {
                    return _DaysSincePreviousDrawing[index];
                }
            }
            return int.MaxValue;
        }

        public void MergeDrawingDatesThenResetDrawingIntervals(int slotId, int number, List<DateTime> dates)
        {
            if (dates.Count < 1) return;

            drawingDates.AddRange(dates);
            List<DateTime> copydrawingDates = drawingDates.OrderBy(i => i).ToList();
            _DaysSincePreviousDrawing.Clear();
            drawingDates.Clear();
            copydrawingDates.ForEach(i => AddDrawingDate(slotId, number, i));
        }

        //public void AddDrawingDates(List<DateTime> dates) => dates.ForEach(i=> AddDrawingDate(i));

        public Number(int Id, int slotId, GameType game, DateTime firstDrawingDate)
        {
            _Id = Id;
            _slotId = slotId;
            _game = game;
            FirstDrawingDate = firstDrawingDate;
        }
    }


}
