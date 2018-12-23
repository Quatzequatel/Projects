using System.Data.SqlClient;
using LotteryV2.Domain.Model;
using System;
using System.Collections.Generic;

namespace LotteryV2.Domain.Extensions
{
    public static class SqlCommandExtensions
    {
        public static List<GetTimesChosenInDateRangeItem> ReadsqlGetTimesChosenInDateRange(this SqlCommand command, DateTime startPeriodDate, int slotId, int period, string game)
        {
            List<GetTimesChosenInDateRangeItem> result = new List<GetTimesChosenInDateRangeItem>();
            SqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                object[] fields = new object[dataReader.FieldCount];
                int fieldCount = dataReader.GetValues(fields);

                result.Add(fields.MapResultToBallTimesChosenInPeriodsDataSetItem()
                .MapToGetTimesChosenInDateRangeItem(startPeriodDate, slotId, period, game));
            }

            if (dataReader != null)
            {
                dataReader.Close();
            }
            return result;
        }

        public static void ExecuteSprocInsertBallTimesChosenInPeriodsDataSet(this SqlCommand command, GetTimesChosenInDateRangeItem item)
        {
            if (item.MapResultToInsertBallTimesChosenInPeriodsDataSet(command).ExecuteNonQuery() < 0)
            {
                //throw new Exception("Error writing to DB."); 
            }
        }
    }
}
