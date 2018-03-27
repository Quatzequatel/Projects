using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public class Slot
    {
        int Id { get; set; }
        private int HighestBall { get; set; }
        public  Numbers Balls { get; private set; }
        public Slot(int id)
        {
            Id = id;
            HighestBall = Utilities.HighBallNumber();
            Balls = new Numbers(HighestBall);
        }

        List<DateTime> DrawingDates { get; set; }

        public string Header()
        {
            return String.Join(", ", Number.Header());
        }

        public string Report()
        {
            return Balls.Report();
        }

        public string VarianceReport()
        {
            return Balls.VarianceReport();
        }

    }
}
