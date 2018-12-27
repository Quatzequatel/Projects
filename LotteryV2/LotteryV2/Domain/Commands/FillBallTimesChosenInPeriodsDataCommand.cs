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
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
        private string sqlGetTimesChosenInDateRange = "SELECT * From [dbo].[GetTimesChosenInDateRange](@Game, @SlotId, @StartDate, @EndDate) ";
        private string sqlInsertBallTimesChosenInPeriodsDataSet = "[dbo].[InsertBallTimesChosenInPeriodsDataSet]";

        private DrawingContext Context;
        private bool IsOpen = false;
        private SqlConnection Connection;
        //private List<int> PeriodDurations = new List<int>() { 3, 5, 8, 13, 21, 34 };
        //private int[] Slots = new int[] { 0, 1, 2, 3, 4 };

        public override bool ShouldExecute(DrawingContext context)
        {
            Context = context;
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            RetrieveDataForAllPeriodsAndSaveSummaryElementsToDB();
        }

        private void RetrieveDataForAllPeriodsAndSaveSummaryElementsToDB()
        {
            int testPeriodDuration = Context.PeriodDurations.Last() * 2;
            DateTime lastDrawingDate= Context.AllDrawings.Where(d => d.Game == Context.GetGameType).OrderByDescending(d => d.DrawingDate).Select(i => i.DrawingDate).First();
            DateTime FirstDrawingDat= lastDrawingDate.AddDays(testPeriodDuration);
            
            DateTime testCaseLastDrawingDate;
            DateTime testCaseFirstDrawingDate;

            int testId = 0;
            int testSampleSize = 2;
            //Get major test case data.
            foreach (var testDate in GetTestDateList(lastDrawingDate, testPeriodDuration, testSampleSize))
            {
                testId++;
                testCaseLastDrawingDate = testDate;
                testCaseFirstDrawingDate = testCaseLastDrawingDate.AddDays(-testPeriodDuration);

                Console.WriteLine($"FillBallTimesChosenInPeriodsDataCommand - Test{testId}");
                Connection = new SqlConnection(connectionString);
                OpenConnection();
                //for each period get data where lastDrawingDate is the same.
                foreach (var period in Context.PeriodDurations)
                {
                    Console.WriteLine($"First Date = {testCaseFirstDrawingDate.ToShortDateString()} to Last Date = {testCaseLastDrawingDate.ToShortDateString()}");
                    Console.WriteLine($"FillBallTimesChosenInPeriodsDataCommand - Period = {period.ToString()}, Slots {Context.Slots[0].ToString()} - {Context.Slots[Context.Slots.Length - 1].ToString()} ");

                    int NumberPeriodsInTestDateRange = GetNumberPeriodsInTestDateRange(testCaseFirstDrawingDate, testCaseLastDrawingDate, period);
                    foreach (var endPeriodDate in GetTestDateList(testCaseLastDrawingDate, period, NumberPeriodsInTestDateRange))
                    {
                        foreach (var slotId in Context.Slots)
                        {
                            List<GetTimesChosenInDateRangeItem> timesChosenList = RetrieveDataForPeriod(testId, endPeriodDate.AddDays(-period).Date, period, slotId);
                            SaveTimeChosenItemToDB(timesChosenList);
                        }
                    }
                }
                CloseConnection();
                Console.WriteLine($"FillBallTimesChosenInPeriodsDataCommand -  Test{testId} completed.");
            }
        }

        private int GetNumberPeriodsInTestDateRange(DateTime startDate, DateTime endDate, int period)
        {
            return Convert.ToInt32(Math.Ceiling((double)(endDate - startDate).Days / Convert.ToDouble(period)));
        }

        private void SaveTimeChosenItemToDB(List<GetTimesChosenInDateRangeItem> timesChosenList)
        {
            foreach (var item in timesChosenList)
            {
                (new SqlCommand(sqlInsertBallTimesChosenInPeriodsDataSet, Connection) { CommandType = System.Data.CommandType.StoredProcedure })
                    .ExecuteSprocInsertBallTimesChosenInPeriodsDataSet(item);
            }
        }



        private List<GetTimesChosenInDateRangeItem> RetrieveDataForPeriod(int testId, DateTime startPeriodDate, int period, int slotId)
        {
            SqlCommand command = new SqlCommand(sqlGetTimesChosenInDateRange, Connection)
                .MapGetBallDrawingsinRangeParameters(Context.GetGameName(),
                                                    slotId,
                                                    startPeriodDate,
                                                    startPeriodDate.AddDays(period).Date);

            return command.ReadsqlGetTimesChosenInDateRange(testId, startPeriodDate, slotId, period, Context.GetGameName());
        }

        private List<DateTime> GetTestDateList(DateTime lastEndDate, int period, int take)
        {
            return Context.AllDrawings.OrderByDescending(d => d.DrawingDate)
                .Where(drawing => drawing.Game == Context.GetGameType && drawing.DrawingDate <= lastEndDate).Select(d => d.DrawingDate)
                .Where((drawing, index) => index % period == 0)
                .Take(take)
                .ToList();
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
    }
}
