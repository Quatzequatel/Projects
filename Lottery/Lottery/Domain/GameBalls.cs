using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public class GameBalls
    {
        private int[] balls = new int[6];
        private string drawingDate = string.Empty;

        public int[] BallNumbers { get => balls; set => balls = value; }
        public string DrawingDate { get => drawingDate; set => drawingDate = value; }
        public DateTime DrawingDateDate { get => DateTime.Parse(drawingDate); }
        public Decimal PrizeAmount { get; set; }
        public int Winners { get; set; }

        public int Chances { get; set; }

        public int SumofBalls => balls.Sum();

        //public string[] Pairs { get => pairs; }
        //public int[] PairSquare { get => pairSquare; }

        private string[] pairs = new string[4];

        private int[] pairSquare = new int[4];

        public void AddBall(int value)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i] == 0)
                {
                    balls[i] = value;
                    break;
                }
            }
        }

        public void AddBall(string value)
        {
            AddBall(Convert.ToInt32(value));
        }
        public string ToCSVString()
        {
            return $"{DrawingDateDate.ToShortDateString()}, {BallNumbers[0]}, {BallNumbers[1]}, {BallNumbers[2]}, {BallNumbers[3]}, {BallNumbers[4]}, {SumofBalls}";
        }

        public static string CSVHeading()
        {
            return "Drawing Date, Balls[0], Balls[1], Balls[2], Balls[3], Balls[4],  SumofBalls";
        }



    }

}
