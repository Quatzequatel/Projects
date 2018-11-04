using System;
using System.Linq;

namespace LotteryV2.Domain.Model
{
    public class HistoricalPeriodItem
    {
        public DateTime DrawingDate { get; set; }
        public int PatternValue { get; set; }

        public SubSets[] Pattern { get; set; }
        public int[] Numbers { get; set; }

        public int NumbersSum { get; set; }
        public HistoricalPeriodItem()
        {

        }

        public HistoricalPeriodItem(DateTime drawingDate, int patternValue, SubSets[] pattern, int[] numbers)
        {
            DrawingDate = drawingDate;
            PatternValue = patternValue;
            Pattern = pattern;
            Numbers = numbers;
            NumbersSum = numbers.Sum();
        }
    }
}
