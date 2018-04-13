using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LotteryV2.Domain
{
    public class Drawing
    {
        private int[] balls = new int[6];
        private string drawingDate = string.Empty;

        [JsonIgnore]
        public Commands.DrawingContext Context { get; private set; }

        public Game Game { get; set; }

        public int[] Numbers { get => balls; set => balls = value; }
        public DateTime DrawingDate { get; set; }
        public Drawing SetDrawingDate(string date) { DrawingDate = DateTime.Parse(date); return this; }
        public Decimal PrizeAmount { get; set; }
        public Drawing SetPrizeAmount(decimal amount) { PrizeAmount = amount; return this; }
        public int Winners { get; set; }
        public Drawing SetWinners(int winners) { Winners = winners; return this; }
        public Drawing SetContext(Commands.DrawingContext context)
        {
            Context = context; Game = context.CurrentGame; return this;
        }
        public Drawing()
        {
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
            if (balls[balls.Length-1] != int.MaxValue)
            {
                balls = balls.OrderBy(i => i).ToArray();
            }
        }

        public void AddBall(string value) => AddBall(Convert.ToInt32(value));

        public string ToCSVString => String.Join(",", new string[]
        {
            $"{Game.ToString()}",
            $"{DrawingDate.ToShortDateString()}",
            $"{Numbers[0]}",
            $"{Numbers[1]}",
            $"{Numbers[2]}",
            $"{Numbers[3]}",
            $"{Numbers[4]}",
            $"{Numbers[5]}"
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
                "Ball-Power"
            });

    }
}
