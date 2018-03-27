using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public class Number
    {
        private int id;
        public int Id { get => id; set => id = value; }
        public int SlotId { get; set; }
        public readonly List<DateTime> DrawingDates = new List<DateTime>();
        public readonly List<int> IntervalDays = new List<int>();
        public readonly LinkedList<Variance> VarianceList = new LinkedList<Variance>();
        public int Chances { get; set; }
        public double IntervalAvg => IntervalDays.Mean();

        public double IntervalSTD => IntervalDays.StandardDeviation();

        public double IntervalSTDLast10 => IntervalDays.LastItems(10).StandardDeviation();

        public double IntervalVariance => IntervalDays.Variance();

        public int IntervalDaysTotal => IntervalDays.Sum();

        public int DrawingsCount => DrawingDates.Count();

        public double DrawingChance => (double)DrawingsCount / Chances;

        public string LastDrawingDate => DrawingsCount > 1 ? DrawingDates[DrawingsCount - 1].ToShortDateString() : "none";

        public string NextLastDrawingDate => DrawingsCount > 2 ? DrawingDates[DrawingsCount - 2].ToShortDateString() : "none";
        public string IsDue => DrawingsCount > 1 ? (DaysSinceLastDrawing > IntervalAvg ? "Yes" : "No") : "N/A";
        public string IsDue2 => DrawingsCount > 1 ? (DaysSinceLastDrawing > IntervalAvg + Math.Abs(IntervalAvg - IntervalSTD) ? "Yes" : "No") : "N/A";
        public int IsDueCount { get; set; }

        public double PickValue => DrawingChance * DaysSinceLastDrawing;
        public double DrawingChancePi => DrawingChance != 0 ? Math.Sqrt(PickValue * Math.PI) : 0;

        public int DaysSinceLastDrawing => DrawingsCount > 1 ? (Scraper.NextDrawingDate.Subtract(DrawingDates[DrawingsCount - 1]).Days) : 0;

        public Number(int id)
        {
            Id = id;
        }

        public void AddDrawing(DateTime date)
        {
            DrawingDates.Add(date);
            if(DrawingDates.Count()>1)
            {
                IntervalDays.Add(DrawingDates[DrawingDates.Count() - 1].Subtract(DrawingDates[DrawingDates.Count() - 2]).Days);
                VarianceList.AddLast(new Variance() { Date = date, Value = IntervalSTDLast10 });
            }
        }

        public static List<string> Header()
        {
            List<string> result = new List<string>
            {
                "SlotId",
                "id",
                "DrawingsCount",
                "IsDue",
                "IsDue2",
                "DrawingChance",
                "DaysSinceLastDrawing",
                "IntervalAvg",
                "IntervalVariance",
                "IntervalSTD",
                "LastDrawingDate",
                "NextLastDrawingDate",
                "IsDueCount",
                "PickValue",
                "DrawingChancePi"
            };
            return result;
        }
        public string Report()
        {
            List<string> result = new List<string>
            {
                $"{SlotId}",
                $"{id}",
                $"{DrawingsCount}",
                $"{IsDue}",
                $"{IsDue2}",
                $"{DrawingChance}",
                $"{DaysSinceLastDrawing}",
                $"{IntervalAvg}",
                $"{IntervalVariance}",
                $"{IntervalSTD}",
                $"{LastDrawingDate}",
                $"{NextLastDrawingDate}",
                $"{IsDueCount}",
                $"{PickValue}",
                $"{DrawingChancePi}"
            };
            return String.Join(",", result);

        }

        public static string VarianceReportHeader()
        {
            List<string> result = new List<string>
            {
                "SlotId",
                "Id",
                "Date",
                "IntervalSTD"
            };
            return String.Join(",", result);
        }

        public string VarianceReport()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var vlItem in VarianceList.OrderByDescending(i=> i.Date).Take(10).ToArray())
            {
                sb.AppendLine($"{SlotId},{id},{vlItem.Date.ToShortDateString()},{vlItem.Value}");
            }
            return sb.ToString();
        }
    }

    public class Variance
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }
}
