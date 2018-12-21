using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Model
{
    public class BallDrawingsInRangeResultItem
    {
        public int RowId { get; private set; }
        
        public int BallId { get; private set; }
        public int SlotId { get; private set; }
        public DateTime DrawingDate { get; private set; }
        public Game Game { get; private set; }
        public BallDrawingsInRangeResultItem(int rowId, int ballId, int slotId, DateTime drawingDate, Game game)
        {
            RowId = rowId;
            BallId = ballId;
            SlotId = slotId;
            DrawingDate = drawingDate;
            Game = game;
        }

        public double RowIdToDouble() => Convert.ToDouble(this.RowId);
        public double BallIdToDouble() => Convert.ToDouble(this.BallId);
        public double SlotIdToDouble() => Convert.ToDouble(this.SlotId);
    }
}
