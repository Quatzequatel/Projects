using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Extensions
{
    public static class OtherExtensions
    {
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            return Enum.TryParse<T>(value, true, out T result) ? result : defaultValue;
        }
    }
}
