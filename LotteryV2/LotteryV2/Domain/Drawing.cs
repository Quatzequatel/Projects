using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LotteryV2.Domain.Commands;

namespace LotteryV2.Domain
{
    public class Drawing
    {
        //Private locals
        private int[] balls;
        private string drawingDate = string.Empty;

        //public properties
        public DateTime DrawingDate { get; set; }
        public Game Game { get; set; }
        /// <summary>
        /// Array of chosen numbers
        /// </summary>
        public int[] Numbers { get => balls; set => balls = value; }
        /// <summary>
        /// value of the jackpot
        /// </summary>
        public Decimal PrizeAmount { get; set; }
        public int Sum => balls.Sum();
        /// <summary>
        /// number of jackpot winners
        /// </summary>
        public int Winners { get; set; }
        /// <summary>
        /// TBD need a better description. Fingerprint is tightly coupled to
        /// period of time. What period is this finger print for?
        /// </summary>
        public FingerPrint TemplateFingerPrint { get; private set; }

        /// <summary>
        /// use HistoricalPeriodFingerPrints for reports and fine tuning of a number selector.
        /// </summary>
        public Dictionary<HistoricalPeriods, FingerPrint> HistoricalPeriodFingerPrints { get; } = new Dictionary<HistoricalPeriods, FingerPrint>();

        public KeyValuePair<string, FingerPrint>[] JsonHistoricalFingerPrints
        {
            get
            {
                return HistoricalPeriodFingerPrints.ToArray()
                    .Select(i => new KeyValuePair<string, FingerPrint>(i.Key.ToString(), i.Value))
                    .ToArray();
            }
            set
            {
                foreach (var item in value)
                {
                    SetHistoricalPeriodFingerPrint((HistoricalPeriods)Enum.Parse(typeof(HistoricalPeriods), item.Key), item.Value);
                }
            }
        }


        //Fluent methods
        public Drawing SetDrawingDate(string date) { DrawingDate = DateTime.Parse(date); return this; }
        public Drawing SetPrizeAmount(decimal amount) { PrizeAmount = amount; return this; }
        public Drawing SetWinners(int winners) { Winners = winners; return this; }


        [JsonIgnore]
        public DrawingContext Context { get; private set; }

        public Drawing()
        {

            balls = new int[new int[0].GetSlotCount()];
            for (int i = 0; i < balls.GetSlotCount(); i++)
            {
                balls[i] = int.MaxValue;
            }
        }

        public void SetHistoricalPeriodFingerPrint(HistoricalPeriods key, FingerPrint value)
        {
            if (!HistoricalPeriodFingerPrints.ContainsKey(key))
            {
                HistoricalPeriodFingerPrints[key] = value;
            }
        }

        public Drawing(DrawingContext context)
        {
            balls = new int[context.SlotCount];
            Context = context; Game = context.GameType;
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i] = int.MaxValue;
            }
        }
        public Drawing SetContext(DrawingContext context)
        {
            Context = context; Game = context.GameType; return this;
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

        public void AddBall(string value) => AddBall(Convert.ToInt32(value));

        /// <summary>
        /// retun the fingerprint for the drawing.
        /// </summary>
        public FingerPrint GetTemplateFingerPrint()
        {
            TemplateFingerPrint = new FingerPrint(this);
            return TemplateFingerPrint;
        }

        public override string ToString()
        {
            return $"{DrawingDate.ToShortDateString()},{KeyString}";
        }

        public string KeyString => string.Join("-", Numbers);

        public string HeadingCSVShort() => $"Game, DrawingDate, Drawing, Sum";

        public string ToCSVShort()
        {
            return String.Join(",", new string[] {$"{Game.ToString()}",
                            $"{DrawingDate.ToShortDateString()}",
                            $"{KeyString}",
                            $"{Sum}" });
        }
        public string ToCSVString()
        {
            switch (Context.SlotCount)
            {
                case 4:
                    return String.Join(",", new string[] {$"{Game.ToString()}",
                            $"{DrawingDate.ToShortDateString()}",
                            $"{Numbers[0]}",
                            $"{Numbers[1]}",
                            $"{Numbers[2]}",
                            $"{Numbers[3]}",
                            $"{Sum}",
                            $"{TemplateFingerPrint}",
                            $"{TemplateFingerPrint.GetValue()}" });
                case 5:
                    return String.Join(",", new string[] {$"{Game.ToString()}",
                            $"{DrawingDate.ToShortDateString()}",
                            $"{Numbers[0]}",
                            $"{Numbers[1]}",
                            $"{Numbers[2]}",
                            $"{Numbers[3]}",
                            $"{Numbers[4]}",
                            $"{Sum}",
                            $"{TemplateFingerPrint}",
                            $"{TemplateFingerPrint.GetValue()}" });
                case 6:
                    return String.Join(",", new string[] {$"{Game.ToString()}",
                            $"{DrawingDate.ToShortDateString()}",
                            $"{Numbers[0]}",
                            $"{Numbers[1]}",
                            $"{Numbers[2]}",
                            $"{Numbers[3]}",
                            $"{Numbers[4]}",
                            $"{Numbers[5]}",
                            $"{Sum}",
                            $"{TemplateFingerPrint}",
                            $"{TemplateFingerPrint.GetValue()}" });
                default:
                    return "";
            }
        }

        public string CSVHeading =>
            Context.SlotCount == 6 ? String.Join(",", new string[]
            {
                "Game",
                "Drawing Date",
                "Ball-1",
                "Ball-2",
                "Ball-3",
                "Ball-4",
                "Ball-5",
                "Odd Ball",
                "Sum",
                "TemplateFingerPrint",
                "TemplateFingerPrint.Value"
            })
            : Context.SlotCount == 5 ? String.Join(",", new string[]
            {
                "Game",
                "Drawing Date",
                "Ball-1",
                "Ball-2",
                "Ball-3",
                "Ball-4",
                "Ball-5",
                "Sum",
                "TemplateFingerPrint",
                "TemplateFingerPrint.Value"
            })
            : String.Join(",", new string[]
            {
                "Game",
                "Drawing Date",
                "Ball-1",
                "Ball-2",
                "Ball-3",
                "Ball-4",
                "Sum",
                "TemplateFingerPrint",
                "TemplateFingerPrint.Value"
            });

    }
}
