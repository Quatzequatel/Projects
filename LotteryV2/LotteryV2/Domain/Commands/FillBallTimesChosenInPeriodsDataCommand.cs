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
            Console.WriteLine("FillBallTimesChosenInPeriodsDataCommand");
            FillTable(context);
            Console.WriteLine("FillBallTimesChosenInPeriodsDataCommand - completed.");
        }

        private void FillTable(DrawingContext context)
        {
            //The first drawing date in table is 8/3/2008.
            //DateTime startDate = new DateTime(2008, 8, 3);
            //use a period of 7 days per data set.
            //int period = 3;
            int[] slots = new int[] { 0, 1, 2, 3, 4 };

            

            Connection = new SqlConnection(connectionString);
            OpenConnection();
            foreach (var period in new int[] {3,5,8,13,21,34 })
            {
                Console.WriteLine($"FillBallTimesChosenInPeriodsDataCommand - Period = {period.ToString()}, Slots {slots[0].ToString()} - {slots[slots.Length - 1].ToString()} ");
                ReadData(context,  period, slots);
            }
            
            CloseConnection();
        }

        private void ReadData(DrawingContext context, int period, int[] slots)
        {
            //DateTime periodStartDate = startDate.Date;
            Console.WriteLine($"Last Date = {GetPeriodDateList( period).First().ToShortDateString()}");
            foreach (var endPeriodDate in GetPeriodDateList( period))
            {
                
                foreach (var slotId in slots)
                {
                    List<GetTimesChosenInDateRangeItem> timesChosenList = RetrieveDataForPeriod(endPeriodDate.AddDays(-period).Date, period, slotId);

                    foreach (var item in timesChosenList)
                    {
                        (new SqlCommand(sqlInsertBallTimesChosenInPeriodsDataSet, Connection) { CommandType = System.Data.CommandType.StoredProcedure })
                            .ExecuteSprocInsertBallTimesChosenInPeriodsDataSet(item);
                    }
                }
                //next period.
                //periodStartDate = dataSetDate.Date;
            }
        }

        private List<GetTimesChosenInDateRangeItem> RetrieveDataForPeriod(DateTime startPeriodDate, int period, int slotId)
        {
            SqlCommand command = new SqlCommand(sqlGetTimesChosenInDateRange, Connection) 
                .MapGetBallDrawingsinRangeParameters(Context.GetGameName(),
                                                    slotId,
                                                    startPeriodDate,
                                                    startPeriodDate.AddDays(period).Date);

            return command.ReadsqlGetTimesChosenInDateRange(startPeriodDate, slotId, period, Context.GetGameName());
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
        private List<DateTime> GetPeriodDateList( int period)
        {
            return Context.AllDrawings.OrderByDescending(d => d.DrawingDate).Where((drawing, index) => drawing.Game == Game.Match4 && index % period == 0).Select(d => d.DrawingDate).ToList();
        }

    }
}
