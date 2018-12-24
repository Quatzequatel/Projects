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
        private List<int> PeriodDurations = new List<int>() { 3, 5, 8, 13, 21, 34 };
        private int[] Slots = new int[] { 0, 1, 2, 3, 4 };
        private DateTime LastDrawingDate() => Context.AllDrawings.Where(d => d.Game == Context.GetGameType).OrderByDescending(d => d.DrawingDate).Select(i => i.DrawingDate).First();
        private DateTime FirstDrawingDate() => LastDrawingDate().AddDays(TestPeriodDuration());

        private DateTime TestCaseLastDrawingDate { get; set; }
        private DateTime TestCaseFirstDrawingDate { get; set; }

        private int TestPeriodDuration() => PeriodDurations.Last() * 2;


        public override bool ShouldExecute(DrawingContext context)
        {
            Context = context;
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
                DoAll();
        }

        private void DoAll()
        {
            int testId = 0;
            int testSampleSize = 2;
            //Get major test case data.
            var testDates1 = GetTestDateList(LastDrawingDate(), TestPeriodDuration(), testSampleSize);
            foreach (var testDate in GetTestDateList(LastDrawingDate(), TestPeriodDuration(), testSampleSize))
            {
                testId++;
                TestCaseLastDrawingDate = testDate;
                TestCaseFirstDrawingDate = TestCaseLastDrawingDate.AddDays(-TestPeriodDuration());

                Console.WriteLine($"FillBallTimesChosenInPeriodsDataCommand - Test{testId}");
                Connection = new SqlConnection(connectionString);
                OpenConnection();
                //for each period get data where lastDrawingDate is the same.
                foreach (var period in PeriodDurations)
                {
                    Console.WriteLine($"FillBallTimesChosenInPeriodsDataCommand - Period = {period.ToString()}, Slots {Slots[0].ToString()} - {Slots[Slots.Length - 1].ToString()} ");

                    Console.WriteLine($"First Date = {TestCaseFirstDrawingDate.ToShortDateString()} to Last Date = {TestCaseLastDrawingDate.ToShortDateString()}");
                    var testDates = GetTestDateList(TestCaseLastDrawingDate, period, GetNumberPeriodsBetween(TestCaseFirstDrawingDate, TestCaseLastDrawingDate, period));

                    foreach (var endPeriodDate in GetTestDateList(TestCaseLastDrawingDate, period, GetNumberPeriodsBetween(TestCaseFirstDrawingDate, TestCaseLastDrawingDate, period)))
                    {
                        foreach (var slotId in Slots)
                        {
                            List<GetTimesChosenInDateRangeItem> timesChosenList = RetrieveDataForPeriod(testId, endPeriodDate.AddDays(-period).Date, period, slotId);
                            WriteTest(timesChosenList);
                        }
                    }

                }

                CloseConnection();
                Console.WriteLine($"FillBallTimesChosenInPeriodsDataCommand -  Test{testId} completed.");
            }
        }

        private int GetNumberPeriodsBetween(DateTime startDate, DateTime endDate, int period)
        {
            return Convert.ToInt32( Math.Ceiling((double)(endDate - startDate).Days / Convert.ToDouble( period)));
        }

        private void WriteTest(List<GetTimesChosenInDateRangeItem> timesChosenList)
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

        private List<DateTime> GetTestDateList(DateTime lastEndDate, int period, int take)
        {
            return Context.AllDrawings.OrderByDescending(d => d.DrawingDate)
                .Where(drawing => drawing.Game == Context.GetGameType && drawing.DrawingDate <= lastEndDate).Select(d=> d.DrawingDate)
                .Where((drawing, index) => index % period == 0)
                .Take(take)
                .ToList();
        }

    }
}
