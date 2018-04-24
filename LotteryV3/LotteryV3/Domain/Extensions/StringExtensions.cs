namespace LotteryV3.Domain.Extensions
{
    public static class  StringExtensions
    {
        public static string CSV(this string[] values)
        {
            return string.Join(",", values);
        }
    }
}
