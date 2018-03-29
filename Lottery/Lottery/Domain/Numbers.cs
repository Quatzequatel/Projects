using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public class Numbers: IEnumerable 
    {
        private Dictionary<int, Number> numberItems = new Dictionary<int, Number>();

        public Numbers(int highestNumber, int slotId)
        {
            for (int i = 1; i <= highestNumber; i++)
            {
                numberItems.Add(i, new Number(i, slotId));
            }
        }

        public string Report()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in numberItems)
            {
                sb.AppendLine(item.Value.Report());
            }
            return sb.ToString();
        }

        public string VarianceReport()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in numberItems)
            {
                sb.Append(item.Value.VarianceReport());
            }
            return sb.ToString();
        }

        public Number this[int key] { get => numberItems[key]; set => numberItems.Add(key, value); }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return numberItems.GetEnumerator();
        }
    }
}
