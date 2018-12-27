using System.Data.SqlClient;
using LotteryV2.Domain.Model;
using System;
using System.Collections.Generic;

namespace LotteryV2.Domain.Extensions
{
    public static class SqlCommandExtensions
    {
        public static List<PeriodSlotIdBallItem> ReadSqlTbl_SelectPeriodCountForSlotIdBallIdSproc(this SqlCommand command, int testId, int slotId, int period, int ballId)
        {
            List<PeriodSlotIdBallItem> result = new List<PeriodSlotIdBallItem>();
            SqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                object[] fields = new object[dataReader.FieldCount];
                int fieldCount = dataReader.GetValues(fields);

                result.Add(fields.MapResultToPeriodSlotIdBallItem(testId, slotId, period, ballId));
            }

            if (dataReader != null)
            {
                dataReader.Close();
            }
            return result;
        }

        public static List<GetTimesChosenInDateRangeItem> ReadsqlGetTimesChosenInDateRange(this SqlCommand command, int testId, DateTime startPeriodDate, int slotId, int period, string game)
        {
            List<GetTimesChosenInDateRangeItem> result = new List<GetTimesChosenInDateRangeItem>();
            SqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                object[] fields = new object[dataReader.FieldCount];
                int fieldCount = dataReader.GetValues(fields);

                result.Add(fields.MapResultToBallTimesChosenInPeriodsDataSetItem()
                .MapToGetTimesChosenInDateRangeItem(testId, startPeriodDate, slotId, period, game));
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

        public static void ExecuteInsertSlopeInterceptDetailsItem(this SqlCommand command, InsertSlopeInterceptDetailsItem item)
        {
            item.MapToExecuteInsertSlopeInterceptDetailsItem(command).ExecuteNonQuery();
        }

        public static Tuple<int?, DateTime, DateTime?> ExecuteGetDaysToNextDrawing(this SqlCommand command)
        {
            int? count = null;
            DateTime startDate = DateTime.MinValue;
            DateTime selectedDate = DateTime.MinValue;

            SqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                object[] fields = new object[dataReader.FieldCount];
                int fieldCount = dataReader.GetValues(fields);

                count = (fields[0] == DBNull.Value ? null : (int?)fields[0]);
                startDate = fields[1] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(fields[1]);
                selectedDate = (fields[2] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(fields[2]));
            }

            if (dataReader != null)
            {
                dataReader.Close();
            }
            return new Tuple<int?, DateTime, DateTime?>(count, startDate, selectedDate);
        }
    }
}
