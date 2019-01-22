using System;
using System.Data.SqlClient;
using System.Linq;
using LotteryV2.Domain.Extensions;
using System.Collections.Generic;
using LotteryV2.Domain.Model;
using MathNet.Numerics;

namespace LotteryV2.Domain.Commands
{
    class InsertSlopeInterceptDetailsItemCommand : Command<DrawingContext>
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
        private string Tbl_SelectPeriodCountForSlotIdBallIdSproc = "Select * from [dbo].[Tbl_SelectPeriodCountForSlotIdBallId](@TestId,@SlotId,@Period,@BallId)";

        private DrawingContext Context;
        private bool IsOpen = false;
        private SqlConnection Connection;
        //private List<int> PeriodDurations = new List<int>() { 3, 5, 8, 13, 21, 34 };
        //private int[] Slots = new int[] { 0, 1, 2, 3, 4 };


        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            Context = context;
            Console.WriteLine("CollectPeriodSlotIdBallTrendCommand");
            CollectData(Context);
            Console.WriteLine("CollectPeriodSlotIdBallTrendCommand - completed.");
        }

        private void CollectData(DrawingContext context)
        {

            for (int testId=1; testId < 1001; testId++)
            {
                Connection = new SqlConnection(connectionString);
                OpenConnection();
                foreach (var period in Context.PeriodDurations)
                {
                    foreach (var slot in Context.Slots)
                    {
                        List<InsertSlopeInterceptDetailsItem> results = new List<InsertSlopeInterceptDetailsItem>();
                        for (int ballId = 1; ballId < Context.HighestBall + 1; ballId++)
                        {
                            var countPeriodResult = Tbl_SelectPeriodCountForSlotIdBallId(testId, slot, period, ballId);
                            Tuple<double, double> InterceptAndSlope= Fit.Line(
                                countPeriodResult.Select(i => i.EndPeriodDateToDouble()).ToArray(), 
                                    countPeriodResult.Select(i => i.CountToDouble())
                                    .ToArray());
                            results.Add(
                                new InsertSlopeInterceptDetailsItem(
                                    countPeriodResult.First().EndPeriodDate, 
                                    period, 
                                    ballId, 
                                    slot,
                                    InterceptAndSlope.Item1,
                                    InterceptAndSlope.Item2,
                                    GetDaysToNextDrawing(countPeriodResult.First().EndPeriodDate,ballId,slot,Context.GetGameName()),
                                    Context.GetGameName()));
                        }
                        InsertSlopeInterceptDetailsItem(results);
                    }
                }
                CloseConnection();
            }
        }

        private List<PeriodSlotIdBallItem> Tbl_SelectPeriodCountForSlotIdBallId(int testId, int slotId, int period, int ballId)
        {
            SqlCommand command = new SqlCommand(Tbl_SelectPeriodCountForSlotIdBallIdSproc, Connection)
                .MapTbl_SelectPeriodCountForSlotIdBallId(testId, slotId, period, ballId);

            return command.ReadSqlTbl_SelectPeriodCountForSlotIdBallIdSproc(testId, slotId, period, ballId);
        }

        private void InsertSlopeInterceptDetailsItem(List<InsertSlopeInterceptDetailsItem> values)
        {
            foreach (var item in values)
            {
                new SqlCommand("[dbo].[InsertSlopeInterceptDetails]", Connection) { CommandType = System.Data.CommandType.StoredProcedure}
                .ExecuteInsertSlopeInterceptDetailsItem(item);
            }
        }

        private int? GetDaysToNextDrawing(DateTime endDrawingDate, int ballId, int slotId, string game)
        {
            var result = (
            new SqlCommand("[dbo].[GetDaysToNextDrawing]", Connection) { CommandType = System.Data.CommandType.StoredProcedure }
            .MapParametersToGetDaysToNextDrawing(endDrawingDate, ballId, slotId, game)
            .ExecuteGetDaysToNextDrawing());

            return result.Item1;
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
