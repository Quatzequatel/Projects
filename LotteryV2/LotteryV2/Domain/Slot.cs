using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LotteryV2.Domain.Commands;

namespace LotteryV2.Domain
{
    public class Slot
    {
        public string Header => string.Join(",", new int[] { 1, 2, 3, 4, 5 });
        public int Id { get; private set; }
        public int MaxNumber { get; private set; }
        public List<Number> Numbers { get; private set; }

        public Slot(DrawingContext context, int slotid)
        {
            MaxNumber = context.HighestBall;
            Numbers = new List<Number>();
            for (int i = 0; i < MaxNumber; i++)
			{
                Numbers.Add(new Number(i, slotid, context.CurrentGame));
			}
        }
    }

}
