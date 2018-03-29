using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public static class Extensions
    {
        public static double Mean(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.Mean(0, values.Count);
        }

        public static double Mean(this List<double> values, int start, int end)
        {
            double s = 0;

            for (int i = start; i < end; i++)
            {
                s += values[i];
            }

            return s / (end - start);
        }

        public static double Variance(this List<double> values)
        {
            return values.Variance(values.Mean(), 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean)
        {
            return values.Variance(mean, 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean, int start, int end)
        {
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / (n);
        }

        public static double StandardDeviation(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.StandardDeviation(0, values.Count);
        }

        public static double StandardDeviation(this List<double> values, int start, int end)
        {
            double mean = values.Mean(start, end);
            double variance = values.Variance(mean, start, end);

            return Math.Sqrt(variance);
        }
        ////////////////////////////////////
        public static double Mean(this List<int> values)
        {
            return values.Count == 0 ? 0 : values.Mean(0, values.Count);
        }

        public static double Mean(this List<int> values, int start, int end)
        {
            double s = 0;

            for (int i = start; i < end; i++)
            {
                s += values[i];
            }

            return s / (end - start);
        }

        public static double Variance(this List<int> values)
        {
            return values.Variance(values.Mean(), 0, values.Count);
        }

        public static double Variance(this List<int> values, double mean)
        {
            return values.Variance(mean, 0, values.Count);
        }

        public static double Variance(this List<int> values, double mean, int start, int end)
        {
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return n > 0 ? variance / (n) : 0;
        }

        public static double StandardDeviation(this List<int> values)
        {
            return values.Count == 0 ? 0 : values.StandardDeviation(0, values.Count);
        }

        public static double StandardDeviation(this List<int> values, int start, int end)
        {
            double mean = values.Mean(start, end);
            double variance = values.Variance(mean, start, end);

            return Math.Sqrt(variance);
        }

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


        public static int LunaPhase(this DateTime date)
        {
            double period = 27.3215;
            DateTime new_moon = new DateTime(1998, 2, 11);
            if (date < new_moon) return -1;
            int days = date.Subtract(new_moon).Days;

            return Convert.ToInt16((days / period - Math.Floor(days / period)) * 27);

        }

        public static string BasicInfo(this GameBalls ball)
        {
            return $"{ball.DrawingDateDate.ToShortDateString()},{ball.DrawingDateDate.DayOfWeek},{String.Join(",", ball.BallNumbers)},{ball.BallsToString()},{ball.Winners},{ball.PrizeAmount.ToString()}";
        }

        public static string BallsToString(this GameBalls ball)
        {
            return String.Join("-", ball.BallNumbers.Select(i => i.ToString("D2")).ToArray());
        }
        //public static Variance TrendValueSum(this Variance)
    }

}
