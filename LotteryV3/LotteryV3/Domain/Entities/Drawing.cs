using System;
using System.Linq;
using Newtonsoft.Json;

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
            if (Context.GetPropabilityGroups == null) { Context.DefineGroups(); }
            if( drawingPattern.Where(i => i != default(PropabilityType)).Count() == 0)
            {
                for (int slotId = 0; slotId < Context.GetBallCount(); slotId++)
                {
                    drawingPattern[slotId] = Context.GetPropabilityGroups.PropabilityTypeForNumber(slotId + 1,
                                                                                                    balls[slotId]);
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

    public class Pattern
    {

    }


}
