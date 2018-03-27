using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public enum Game
    {
        Lotto,
        MegaMillion,
        Powerball,
        Hit5
    }

    public class Drawing
    {

        public Game Type { get => Scraper.CurrentGame; }
        public int SlotCount { get => Utilities.GetBallCount(); }
        public int HighestBall { get => Utilities.HighBallNumber(); }
        
        public List<DateTime> DrawingDates { get; set; }

        public Slots Slots;
        public Drawing()
        {
            Slots = new Slots(SlotCount, HighestBall);
        }

        public void AddDrawing(object data)
        {
            new Exception("TBD");
        }

        public StringBuilder GetPicks(int PickCount)
        {
            StringBuilder sb = new StringBuilder();
            return sb;
        }

        public override string ToString()
        {
            new Exception("TBD");
            return "TBD";
        }
    }

    public struct PossibleNumbers
    {
        public int Index;
        public string Balls;
        public DateTime? ChosenOn1;
        public DateTime? ChosenOn2;
    }
}
