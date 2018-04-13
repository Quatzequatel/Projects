using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain
{
    public static class Extensions
    {
        public static string CSV(this string[] values)
        {
            return string.Join(",", values);
        }
    }
}
