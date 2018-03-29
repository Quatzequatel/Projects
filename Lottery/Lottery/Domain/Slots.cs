using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public class Slots: IEnumerable
    {
        private Dictionary<int, Slot> slotList = new Dictionary<int, Slot>();
        public Slots()
        {

        }
        public Slots(int size, int highestNumber)
        {
            for (int i = 0; i < size; i++)
            {
                slotList.Add(i, new Slot(i));
            }
        }

        public Slot this[int key]
        {
            get
            {
                return slotList[key];
            }
            set
            {
                slotList.Add(key, value);
            }
        }

        public void AddDrawing(List<GameBalls> Drawings)
        {
            int i = 0;
            foreach (var drawing in Drawings)
            {
                i++;
                AddDrawing(drawing);
            }
            //update stats.
        }
        public void AddDrawing(GameBalls drawing)
        {
            for (int slotId = 0; slotId < drawing.BallNumbers.Length; slotId++)
            {
                AddDrawing(slotId, drawing.BallNumbers[slotId], drawing.DrawingDateDate);
            }
        }
        public void AddDrawing(int slotId, int ballId, DateTime drawingDate)
        {
            slotList[slotId].Balls[ballId].AddDrawing(drawingDate);
        }

        void SetIsDueCount()
        {

        }

        public string Report() => StandardTable();

        public string StandardTable()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(slotList.First().Value.Header());
            foreach (var slot in slotList)
            {
                sb.Append(slot.Value.Report());
            }

            return sb.ToString();
        }

        public string VarianceReport()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Number.VarianceReportHeader());
            foreach (var slot in slotList)
            {
                sb.Append(slot.Value.VarianceReport());
            }

            return sb.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return slotList.GetEnumerator();
        }

        public class Picks
        {
            Slots Slots { get; set; }
            Game Game { get; set; }
            public Picks(Slots slots, Game game)
            {
                Slots = slots;
                Game = game;
            }

        }

    }
}
