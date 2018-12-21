using System;
using System.Data.SqlClient;
using System.Linq;
using LotteryV2.Domain.Extensions;
using System.Collections.Generic;
using LotteryV2.Domain.Model;
// using MathNet.Numerics;

namespace LotteryV2.Domain.Commands
{
    class FillBallTimesChosenInPeriodsDataCommand : Command<DrawingContext>
    {
        private DrawingContext Context;
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
        private string sqlGetTimesChosenInDateRange = "SELECT * From [dbo].[GetTimesChosenInDateRange](@Game, @SlotId, @StartDate, @EndDate) ORDER BY [COUNT] DESC";
        private string sqlInsertBallTimesChosenInPeriodsDataSet = "[dbo].[InsertBallTimesChosenInPeriodsDataSet]";
        /*
         * Parameters are:
        @StartDate = ,
		@Period = NULL,
		@BallId = NULL,
		@SlotId = NULL,
		@Count = NULL,
		@Percent = NULL,
		@Game = NULL

             */

        public override bool ShouldExecute(DrawingContext context)
        {
            Context = context;
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("TrendExpirementCommand");
            FillTable(context);
            Console.WriteLine("TrendExpirementCommand - completed.");
        }

        private void FillTable(DrawingContext context)
        {
            //The first drawing date in table is 8/3/2008.
            DateTime startDate = new DateTime(2008, 8, 3);
            //use a period of 7 days per data set.
            int period = 7;
            int[] slots = new int[] { 0, 1, 2, 3, 4 };

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                bool isOpen = false;

                foreach (var item in GetPeriodDateList(startDate, period))
                {
                    SqlDataReader dataReader = null;
                    try
                    {
                        using (SqlCommand command = new SqlCommand(sqlGetTimesChosenInDateRange, con))
                        {
                            if(isOpen)
                            {
                                isOpen = true;
                                con.Open();
                            }

                            foreach (var slotId in slots)
                            {
                                dataReader = command.MapGetBallDrawingsinRangeParameters(context.GetGameName(),
                                                          0,
                                                          item,
                                                          item.AddDays(period)).ExecuteReader();
                                while(dataReader.Read())
                                {
                                    object[] fields = new object[dataReader.FieldCount];
                                    int fieldCount = dataReader.GetValues(fields);

                                    GetTimesChosenInDateRangeItem dataSetItem
                                        = fields.MapResultToBallTimesChosenInPeriodsDataSetItem()
                                        .MapToGetTimesChosenInDateRangeItem(startDate, period, context.GetGameName());

                                    using (SqlCommand command2 = new SqlCommand(sqlInsertBallTimesChosenInPeriodsDataSet, con) { CommandType = System.Data.CommandType.StoredProcedure })
                                    {
                                        if(dataSetItem.MapResultToInsertBallTimesChosenInPeriodsDataSet(command2).ExecuteNonQuery() < 0)
                                        {
                                            //throw new Exception("Error writing to DB."); 
                                        }
                                    }
                                }
                            }

                        }
                }
                    finally
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Select dates at period intervals.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        private List<DateTime> GetPeriodDateList(DateTime startDate, int period)
        {
            return Context.AllDrawings.Where((drawing, index) => (index + 1) % period == 0).Select(d => d.DrawingDate).ToList();
        }

    }
}
