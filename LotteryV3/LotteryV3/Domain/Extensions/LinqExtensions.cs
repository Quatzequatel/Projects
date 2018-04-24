using System.Collections.Generic;

namespace LotteryV3.Domain.Extensions
{
    public static class LinqExtensions
    {
        public static List<int> LastItems(this List<int> values, int take)
        {
            List<int> LastXList = new List<int>();
            int start = 0;
            if (values.Count > take)
            {
                start = values.Count - take;
            }
            else
            {
                take = values.Count;
            }

            for (int i = start; i < start + take; i++)
            {
                LastXList.Add(values[i]);
            }
            return LastXList;
        }
    }
}
