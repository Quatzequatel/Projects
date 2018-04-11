using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain
{
    public class Drawing
    {
        private int[] balls = new int[6];
        private string drawingDate = string.Empty;
        public int[] Numbers { get => balls; }
        public DateTime DrawingDate { get; private set; }
        public Drawing SetDrawingDate(string date) { DrawingDate = DateTime.Parse(date); return this; }
        public Decimal PrizeAmount { get; private set; }
        public Drawing SetPrizeAmount(decimal amount) { PrizeAmount = amount; return this; }
        public int Winners { get; private set; }
        public Drawing SetWinners(int winners) { Winners = winners; return this; }

        public void AddBall(int value)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i] == 0) { balls[i] = value; break; }
            }
        }

        public void AddBall(string value) => AddBall(Convert.ToInt32(value));

        public string ToCSVString => $"{DrawingDate.ToShortDateString()},{Numbers[0]},{Numbers[1]},{Numbers[2]},{Numbers[3]},{Numbers[4]},{Numbers[5]}";
        public string CSVHeading => "DrawingDate.ToShortDateString(),Numbers[0],Numbers[1],Numbers[2],Numbers[3],Numbers[4],Numbers[5]";

    }
}
