using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV3.Domain.Entities
{
    public class TrendGroups
    {
        public GameType Game { get; set; }
        private DrawingContext Context;

    }

    public class TrendGroup
    {
        public readonly int SlotId;
        public readonly GameType Game;
        public readonly PropabilityType Propability;


        private List<NumberInfo> _NumberSet;
        public List<NumberInfo> NumberSet => _NumberSet;

        public TrendGroup(GameType game, int slotId, PropabilityType propability)
        {
            SlotId = slotId;
            Game = game;
            Propability = propability;
        }
        public void AddNumbers(List<NumberInfo> values)
        {

        }
    }

    public class TrendValue
    {
        /// <summary>
        /// Date of drawing.
        /// </summary>
        DateTime DrawingDate { get; set; }
        /// <summary>
        /// Slot or Ball index, note this is 1 based; 
        /// Slot zero is reserved for the number summary.
        /// </summary>
        public int SlotId { get; set; }
        /// <summary>
        /// the number drawn.
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// number of days since previous drawing.
        /// </summary>
        public int Interval { get; set; }

        public TrendValue(int slotId, int number, int interval, DateTime drawingDate)
        {
            SlotId = slotId;
            Number = number;
            Interval = interval;
            DrawingDate = drawingDate;
        }

        public TrendValue(int slotId, int number, DateTime drawingDate, DateTime? previousDrawingDate, DateTime firstDrawingDate)
            : this(slotId,
                 number,
                 (previousDrawingDate.HasValue ? drawingDate.Subtract(previousDrawingDate.Value).Days : drawingDate.Subtract(firstDrawingDate).Days),
                 drawingDate)
        { }
    }
}
