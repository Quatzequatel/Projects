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
        private string sqlGetTimesChosenInDateRange = "SELECT * From [dbo].[GetTimesChosenInDateRange](@Game, @SlotId, @StartDate, @EndDate) ";
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

        private bool IsOpen = false;
        private SqlConnection Connection;

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

            Connection = new SqlConnection(connectionString);
            OpenConnection();
            ReadData(context, startDate, period, slots);
            CloseConnection();
        }

        private void ReadData(DrawingContext context, DateTime startDate, int period, int[] slots)
        {
            DateTime periodStartDate = startDate.Date;
            foreach (var dataSetDate in GetPeriodDateList(startDate, period))
            {
                foreach (var slotId in slots)
                {
                    List<GetTimesChosenInDateRangeItem> timesChosenList = RetrieveDataForPeriod(periodStartDate, dataSetDate, period, slotId);

                    foreach (var item in timesChosenList)
                    {
                        (new SqlCommand(sqlInsertBallTimesChosenInPeriodsDataSet, Connection) { CommandType = System.Data.CommandType.StoredProcedure })
                            .ExecuteSprocInsertBallTimesChosenInPeriodsDataSet(item);
                    }
                }
                //next period.
                periodStartDate = dataSetDate.Date;
            }
        }

        private List<GetTimesChosenInDateRangeItem> RetrieveDataForPeriod(DateTime startDate, DateTime dataSetDate, int period, int slotId)
        {
            SqlCommand command = new SqlCommand(sqlGetTimesChosenInDateRange, Connection) 
                .MapGetBallDrawingsinRangeParameters(Context.GetGameName(),
                                                    slotId,
                                                    dataSetDate,
                                                    dataSetDate.AddDays(period));

            return command.ReadsqlGetTimesChosenInDateRange(startDate, slotId, period, Context.GetGameName());
        }


        private void OpenConnection()
        {
            if (!IsOpen && Connection != null)
            {
                IsOpen = true;
                Connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (Connection != null)
            {
                Connection.Close();
                IsOpen = false;
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
