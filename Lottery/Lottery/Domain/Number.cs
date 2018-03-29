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
        public readonly LinkedList<Variance> Variance10dayList = new LinkedList<Variance>();
        public readonly List<int> LunaPhases = new List<int>();
        public int Chances { get; set; }
        public double IntervalAvg => IntervalDays.Mean();

        public double IntervalSTD => IntervalDays.StandardDeviation();

        public double IntervalSTDLast10 => IntervalDays.LastItems(10).StandardDeviation();
        public int IntervalTotalLast10Days => IntervalDays.LastItems(10).Sum();

        public double IntervalVariance => IntervalDays.Variance();

        public int IntervalDaysTotal => IntervalDays.Sum();

        public double LunaMean => LunaPhases.Mean();
        public double LunaSTD => LunaPhases.StandardDeviation();
        public double LunaVariance => LunaPhases.Variance();

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

        public double TrendValueSum => Variance10dayList.Sum(i => i.TrendValue);

        public int MostSelectedLunaPhase()
        {
            Dictionary<int, int> lunaDays = new Dictionary<int, int>();
            for (int i = 1; i < 25; i++)
            {
                lunaDays[i] = LunaPhases.Where(phase => phase == i).Count();
            }

            Tuple<int, int> max = new Tuple<int, int>(0, 0);
            for (int i = 0; i < 25; i++)
            {
                if (lunaDays[i] > max.Item2) max = new Tuple<int, int>(i, lunaDays[i]);
            }
            return max.Item1;
        }

        public Number(int id, int slotId)
        {
            Id = id;
            SlotId = slotId;
        }

        public void AddDrawing(DateTime date)
        {
            DrawingDates.Add(date);
            LunaPhases.Add(date.LunaPhase());
            if (DrawingDates.Count() > 1)
            {
                IntervalDays.Add(DrawingDates[DrawingDates.Count() - 1].Subtract(DrawingDates[DrawingDates.Count() - 2]).Days);

                VarianceList.AddLast(new Variance() { Date = date, IntervalSTD = IntervalSTD });
                Variance10dayList.AddLast(new Variance()
                {
                    Date = date,
                    IntervalSTD = IntervalSTD,
                    IntervalSTDLast10 = IntervalSTDLast10,
                    IntervalSum = IntervalTotalLast10Days,
                    TrendValue = Variance10dayList.Count > 1 ? (Variance10dayList.Last.Value.IntervalSTDLast10 - Variance10dayList.Last.Previous.Value.IntervalSTDLast10) : 0.0
                });
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
                "TrendValueSum",
                "LastDrawingDate",
                "NextLastDrawingDate",
                "IsDueCount",
                "PickValue",
                "DrawingChancePi",
                "LunaMean",
                "LunaSTD",
                "LunaVariance"
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
                $"{TrendValueSum}",
                $"{LastDrawingDate}",
                $"{NextLastDrawingDate}",
                $"{IsDueCount}",
                $"{PickValue}",
                $"{DrawingChancePi}",
                $"{LunaMean}",
                $"{LunaSTD}",
                $"{LunaVariance}"
            };
            return String.Join(",", result);

        }

        public static string VarianceReportHeader()
        {
            List<string> result = new List<string>
            {
                "SlotId",
                "Number",
                "Index",
                "Date",
                "IntervalSTD",
                "IntervalSTDLast10",
                "IntervalSum",
                "TrendValue"
            };
            return String.Join(",", result);
        }

        public string VarianceReport()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var vlItem in Variance10dayList.OrderByDescending(d => d.Date).Take(10).ToArray())
            {
                i++;
                sb.AppendLine($"{SlotId},{id},{i},{vlItem.Date.ToShortDateString()},{vlItem.IntervalSTD},{vlItem.IntervalSTDLast10},{vlItem.IntervalSum},{ vlItem.TrendValue}");
            }

            return sb.ToString();
        }
    }

    public class Variance
    {
        public DateTime Date { get; set; }
        public double IntervalSTD { get; set; }
        public double IntervalSTDLast10 { get; set; }
        public int IntervalSum { get; set; }
        public double TrendValue { get; set; }
    }
}
