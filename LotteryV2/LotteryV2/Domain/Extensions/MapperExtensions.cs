﻿using System;
using LotteryV2.Domain.Model;

namespace LotteryV2.Domain.Extensions
{
    public static class MapperExtensions
    {
        public static double[] MapGetBallDrawingsInRangeResults(this object[] fields)
        {
            return new double[] { Convert.ToDouble(fields[0]), Convert.ToDouble(fields[1]), Convert.ToDouble(fields[2]) };
        }

        public static BallDrawingsInRangeResultItem MapResultToBallDrawingsInRangeResultItem(this object[] fields)
        {
            return new BallDrawingsInRangeResultItem(Convert.ToInt16(fields[0]),
                                                     Convert.ToInt16(fields[1]),
                                                     Convert.ToInt16(fields[2]),
                                                     Convert.ToDateTime(fields[3]),
                                                     OtherExtensions.ToEnum<Game>(fields[4].ToString(), Game.Match4)
                                                     );
        }

        public static BallTimesChosenInPeriodsDataSetItem MapResultToBallTimesChosenInPeriodsDataSetItem(this object[] fields)
        {
            return new BallTimesChosenInPeriodsDataSetItem(Convert.ToInt16(fields[0]),
                                         Convert.ToInt16(fields[1]),
                                         Convert.ToString(fields[2])
                                         );
        }

        public static GetTimesChosenInDateRangeItem MapToGetTimesChosenInDateRangeItem(this BallTimesChosenInPeriodsDataSetItem item,
                                                    DateTime startDate, int period, string game)
        {
            return new GetTimesChosenInDateRangeItem(item, startDate, period, game);
        }
    }
}
