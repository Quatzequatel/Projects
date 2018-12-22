using System;
using System.Data.SqlClient;
using System.Linq;
using LotteryV2.Domain.Extensions;
using System.Collections.Generic;
using LotteryV2.Domain.Model;
using MathNet.Numerics;

namespace LotteryV2.Domain.Commands
{
    class TrendExpirementCommand : Command<DrawingContext>
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
        private string SprocName = "SELECT * From [dbo].[GetBallDrawingsInRange](@Game, @SlotId, @StartDate, @EndDate) ";

        public override bool ShouldExecute(DrawingContext context)
        {
            //return base.ShouldExecute(context);
            return true;
        }

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("TrendExpirementCommand");
            DoExpiriment(context);
            Console.WriteLine("TrendExpirementCommand - completed.");
        }

        private void DoExpiriment(DrawingContext context)
        {
            List<double> rowIds = new List<double>();
            List<double> ballIds = new List<double>();
            List<BallDrawingsInRangeResultItem> results = new List<BallDrawingsInRangeResultItem>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                bool isopen = false;
                DateTime startDate = DateTime.Today.AddDays(-45);
                DateTime ExpirementStartDate = startDate; //TBD move into context?

                foreach (var item in context.AllDrawings.Where(i => i.DrawingDate > ExpirementStartDate && i.DrawingDate < DateTime.Today.AddDays(-14)))
                {
                    SqlDataReader dataReader = null;

                    try
                    {
                        using (SqlCommand command = new SqlCommand(SprocName, con))
                        {
                            if (!isopen)
                            {
                                isopen = true;
                                con.Open();
                            }

                            dataReader = command.MapGetBallDrawingsinRangeParameters(context.GetGameName(), 1, item.DrawingDate, item.DrawingDate.AddDays(14)).ExecuteReader();
                            Console.WriteLine($"{command.CommandText} - '{context.GetGameName()}, 1, '{item.DrawingDate.ToShortDateString()}', '{item.DrawingDate.AddDays(14).ToShortDateString()}'" );
                            while (dataReader.Read())
                            {
                                //if(dataReader.FieldCount == DBNull)
                                object[] fields = new object[dataReader.FieldCount];
                                int fieldCount = dataReader.GetValues(fields);
                                results.Add(fields.MapResultToBallDrawingsInRangeResultItem());
                                rowIds.Add(results.Last().RowIdToDouble());
                                ballIds.Add(results.Last().BallIdToDouble());
                            }

                            // Regression?

                            Tuple<double, double> p = Fit.Line(ballIds.ToArray(), rowIds.ToArray());
                            
                            Console.WriteLine($"Intercept = {p.Item1}, slope = {p.Item2}");
                            double nextBall = (results.Last().RowId + 1 - p.Item2) / p.Item1;
                            Console.WriteLine();
                            Console.WriteLine($"Next ball = {Convert.ToInt16(nextBall)} from {nextBall.ToString()}");
                            Console.WriteLine();
                        }
                    }
                    finally
                    {
                        if (dataReader != null)
                        {
                            dataReader.Close();
                        }
                    }
                }

                if (con != null)
                {
                    con.Close();
                    isopen = false;
                }
            }

        }
    }
}
