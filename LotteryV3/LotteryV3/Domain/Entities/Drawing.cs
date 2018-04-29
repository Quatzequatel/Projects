using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using LotteryV3.Domain.Extensions;
using LotteryV3.Domain;

namespace LotteryV3.Domain.Entities
{
    public class Drawing
    {
        [JsonIgnore]
        public DrawingContext Context { get; private set; }
        private int[] balls = new int[6];
        private PropabilityType[] drawingPattern;

        public PropabilityType[] GetDrawingPattern()
        {
            if( drawingPattern.Where(i => i != default(PropabilityType)).Count() == 0)
            {
                for (int slotId = 0; slotId < Context.GetBallCount(); slotId++)
                {
                    drawingPattern[slotId] = Context.GetPropabilityGroups.PropabilityTypeForNumber(slotId+1, balls[slotId]);
                }
            }
            return drawingPattern;
        }

        private void SetDrawingPattern(PropabilityType[] value)=> drawingPattern = value;
        
        public TrendType[] TrendPattern { get; private set; }

        private string drawingDate = string.Empty;
        public DateTime DrawingDate { get; set; }
        public GameType Game { get; set; }
        public Decimal PrizeAmount { get; set; }
        public int[] Numbers { get => balls; set => balls = value; }
        public int Sum => balls.Sum();
        public int Winners { get; set; }


        public Drawing SetDrawingDate(string date) { DrawingDate = DateTime.Parse(date); return this; }
        public Drawing SetPrizeAmount(decimal amount) { PrizeAmount = amount; return this; }
        public Drawing SetWinners(int winners) { Winners = winners; return this; }

        public void AddBall(string value) => AddBall(Convert.ToInt32(value));

        internal Drawing SetContext(DrawingContext context)
        {
            Context = context;
            Game = context.Game;
            //balls = new int[context.GetBallCount()];
            SetDrawingPattern(new PropabilityType[context.GetBallCount()]);
            TrendPattern = new TrendType[context.GetBallCount()];

            return this;
        }

        public Drawing(DrawingContext context)
        {
            if (context != null) { SetContext(context); }
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i] = int.MaxValue;
            }
        }
        public void AddBall(int value)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i] == int.MaxValue) { balls[i] = value; break; }
            }
            if (balls[balls.Length - 1] != int.MaxValue)
            {
                balls = balls.OrderBy(i => i).ToArray();
            }
        }

        public TrendValue GetTrendValue(int slotId)
        {
            return Context.TrendDictionary.ContainsKey(new TrendKey(slotId, DrawingDate).ToString()) 
                ? Context.TrendDictionary[new TrendKey(slotId, DrawingDate).ToString()]
                : new TrendValue(slotId, -1, 0, DrawingDate);
        }
        public override string ToString()
        {
            return $"{DrawingDate.ToShortDateString()},{Numbers[0]}-{Numbers[1]}-{Numbers[2]}-{Numbers[3]}-{Numbers[4]}-{Numbers[5]}";
        }

        public string KeyString => string.Join("-", Numbers);

        public string ToCSVString => String.Join(",", new string[]
        {
            $"{Game.ToString()}",
            $"{DrawingDate.ToShortDateString()}",
            $"{Numbers[0]}",
            $"{Numbers[1]}",
            $"{Numbers[2]}",
            $"{Numbers[3]}",
            $"{Numbers[4]}",
            $"{Numbers[5]}",
            $"{Sum}",
            $"{GetDrawingPattern()[0]}",
            $"{drawingPattern[1]}",
            $"{drawingPattern[2]}",
            $"{drawingPattern[3]}",
            $"{drawingPattern[4]}",
            $"{drawingPattern[5]}",
            $"{GetTrendValue(0).Interval}",
            $"{GetTrendValue(1).Interval}",
            $"{GetTrendValue(2).Interval}",
            $"{GetTrendValue(3).Interval}",
            $"{GetTrendValue(4).Interval}",
            $"{GetTrendValue(5).Interval}",
        });
        public string CSVHeading =>
            String.Join(",", new string[]
            {
                "Game",
                "Drawing Date",
                "Ball-1",
                "Ball-2",
                "Ball-3",
                "Ball-4",
                "Ball-5",
                "Ball-Power",
                "Sum",
                "Propability 1",
                "Propability 2",
                "Propability 3",
                "Propability 4",
                "Propability 5",
                "Propability 6",
                "TrendValue 1",
                "TrendValue 2",
                "TrendValue 3",
                "TrendValue 4",
                "TrendValue 5",
                "TrendValue 6",
            });
    }

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

    /// <summary>
    /// NumberInfo builds on number storing more analysis data on a given slot:ball pair and can hold data in 
    /// relationship to other numbers for given slot.
    /// </summary>
    public class NumberInfo : Number
    {
        public int TotalSum;
        public double AvgSum;
        public int MinSum;
        public int MaxSum;

        public double SumSTD;
        public double SumVariance;

        public PropabilityType GroupType;

        public int TimesChosen => DrawingDates.Count;
        public int DrawingsCount { get; private set; }
        public void SetDrawingsCount(int value) => DrawingsCount = value;

        public double PercentChosen => ((double)TimesChosen / DrawingsCount) * 100;

        public NumberInfo(int Id, int slotId, GameType game, DateTime firstDrawingDate) : base(Id, slotId, game, firstDrawingDate)
        {

        }

        public void LoadDrawings(List<Drawing> drawings)
        {
            var list = drawings.Where(drawing => drawing.Game == Game && drawing.Numbers[SlotId - 1] == Id).ToArray();
            foreach (var item in list)
            {
                base.AddDrawingDate(SlotId, item.Numbers[SlotId - 1], item.DrawingDate);
                TotalSum = +item.Sum;
            }
            DrawingsCount = drawings.Count;
            if (list.Count() == 0) return;
            MinSum = list.Select(i => i.Sum).Min();
            MaxSum = list.Select(i => i.Sum).Max();
            AvgSum = list.Select(i => i.Sum).Average();
            SumSTD = list.Select(i => i.Sum).ToList().StandardDeviation();
            SumVariance = list.Select(i => i.Sum).ToList().Variance();
        }



        public string CSVHeading => new string[] { "Game", "Slot", "Number", "Times Chosen", "Chosen %", "AvgSum", "MinSum", "MaxSum", "SumSTD", "SumVariance" }.CSV();
        public string CSVLine => new string[]
        {
            $"{Game}",
            $"{SlotId}",
            $"{Id}",
            $"{TimesChosen}",
            $"{PercentChosen}",
            $"{AvgSum}",
            $"{MinSum}",
            $"{MaxSum}",
            $"{SumSTD}",
            $"{SumVariance}"
        }.CSV();
    }

    public class NumberModel : Number
    {
        public int TotalSum;
        public double AvgSum;
        public int MinSum;
        public int MaxSum;

        public double SumSTD;
        public double SumVariance;

        public PropabilityGroup Group;

        public int TimesChosen => DrawingDates.Count;
        public double PercentChosen => ((double)TimesChosen / DrawingsCount) * 100;

        public decimal TrendlineYvalue { get; set; }

        public int DrawingsCount { get; private set; }
        public void SetDrawingsCount(int value) => DrawingsCount = value;

        public NumberModel(int id, int slotid, GameType game, DateTime firstDrawingDate) : base(id, slotid, game, firstDrawingDate) { }
        public void LoadDrawingsRange(List<Drawing> drawings, DateTime start, DateTime end)
        {
            LoadDrawings(drawings.Where(i => i.DrawingDate >= start && i.DrawingDate <= end).ToList());
        }
        public void LoadLastNumberOfDrawingsAndLeave(List<Drawing> drawings, int PreviousDrawingsCount, int LeaveDrawingCount)
        {
            int TakeCount = (PreviousDrawingsCount + LeaveDrawingCount) > drawings.Count ? drawings.Count - LeaveDrawingCount : PreviousDrawingsCount;

            LoadDrawings(drawings.OrderByDescending(i => i.DrawingDate).Take(TakeCount + LeaveDrawingCount).Skip(LeaveDrawingCount).ToList());
        }
        /// <summary>
        /// add all drawing dates when number was selected in given slot.
        /// </summary>
        /// <param name="drawings">list of drawings to define analysis pool.</param>
        public void LoadDrawings(List<Drawing> drawings)
        {
            var list = drawings.Where(drawing => drawing.Game == Game && drawing.Numbers[SlotId - 1] == Id).ToArray();
            foreach (var item in list)
            {
                base.AddDrawingDate(SlotId, item.Numbers[SlotId - 1], item.DrawingDate);
                TotalSum = +item.Sum;
            }
            DrawingsCount = drawings.Count;
            if (list.Count() == 0) return;
            MinSum = list.Select(i => i.Sum).Min();
            MaxSum = list.Select(i => i.Sum).Max();
            AvgSum = list.Select(i => i.Sum).Average();
            SumSTD = list.Select(i => i.Sum).ToList().StandardDeviation();
            SumVariance = list.Select(i => i.Sum).ToList().Variance();

        }
    }

    public class Pattern
    {

    }


}
