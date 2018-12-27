using System.Data.SqlClient;
using LotteryV2.Domain.Model;
using LotteryV2.Domain.Extensions;
using System;

namespace LotteryV2.Domain.Extensions
{
    public static class DatabaseExtensions
    {
        public static SqlCommand MapParametersToGetDaysToNextDrawing(this SqlCommand sqlCommand, DateTime endDrawingDate, int ballId, int slotId, string game)
        {
            sqlCommand.Parameters.AddWithValue("@EndDrawingDate", endDrawingDate);
            sqlCommand.Parameters.AddWithValue("@BallId", ballId);
            sqlCommand.Parameters.AddWithValue("@SlotId", slotId);
            sqlCommand.Parameters.AddWithValue("@Game", game);

            return sqlCommand;
        }
        public static SqlCommand MapTbl_SelectPeriodCountForSlotIdBallId(this SqlCommand sqlCommand, int testId, int slotId, int period, int ballId)
        {
            sqlCommand.Parameters.AddWithValue("@TestId", testId);
            sqlCommand.Parameters.AddWithValue("@SlotId", slotId);
            sqlCommand.Parameters.AddWithValue("@Period", period);
            sqlCommand.Parameters.AddWithValue("@BallId", ballId);


            return sqlCommand;
        }
        public static SqlCommand MapGetBallDrawingsinRangeParameters(this SqlCommand sqlcommand, string game, int slotId, DateTime startDate, DateTime endDate)
        {
            sqlcommand.Parameters.AddWithValue("@Game", game);
            sqlcommand.Parameters.AddWithValue("@SlotId", slotId.ToString());
            sqlcommand.Parameters.AddWithValue("@StartDate", startDate.ToShortDateString());
            sqlcommand.Parameters.AddWithValue("@EndDate", endDate.ToShortDateString());
            

            return sqlcommand;
        }

        public static SqlCommand MapResultToInsertBallTimesChosenInPeriodsDataSet(this GetTimesChosenInDateRangeItem item, SqlCommand sqlCommand)
        {
            sqlCommand.Parameters.AddWithValue("@TestId", item.TestId);
            sqlCommand.Parameters.AddWithValue("@EndPeriodDate", item.EndPeriodDate);
            sqlCommand.Parameters.AddWithValue("@Period", item.Period);
            sqlCommand.Parameters.AddWithValue("@BallId", item.BallId);
            sqlCommand.Parameters.AddWithValue("@SlotId", item.SlotId);
            sqlCommand.Parameters.AddWithValue("@Count", item.Count);
            sqlCommand.Parameters.AddWithValue("@Percent", item.Percent);
            sqlCommand.Parameters.AddWithValue("@Game", item.Game.GetName(typeof(Game)));

            return sqlCommand;
        }

        public static SqlCommand MapToExecuteInsertSlopeInterceptDetailsItem(this InsertSlopeInterceptDetailsItem item, SqlCommand sqlCommand)
        {
            sqlCommand.Parameters.AddWithValue("@EndPeriodDate", item.EndPeriodDate);
            sqlCommand.Parameters.AddWithValue("@Period", item.Period);
            sqlCommand.Parameters.AddWithValue("@BallId", item.BallId);
            sqlCommand.Parameters.AddWithValue("@SlotId", item.SlotId);
            sqlCommand.Parameters.AddWithValue("@Intercept", item.Intercept);
            sqlCommand.Parameters.AddWithValue("@Slope", item.Slope);
            sqlCommand.Parameters.Add("@NextTimeChosenCount", System.Data.SqlDbType.Int);
            
            if (item.NextTimeChosenCount.HasValue)
            {
                sqlCommand.Parameters["@NextTimeChosenCount"].Value = item.NextTimeChosenCount.Value;
            }
            else
            {
                sqlCommand.Parameters["@NextTimeChosenCount"].Value = DBNull.Value;
            }

            sqlCommand.Parameters.AddWithValue("@Game", item.Game.GetName(typeof(Game)));

            return sqlCommand;
        }

        public static SqlCommand MapDrawingToInsertBallDrawingDetails(this Drawing item, SqlCommand sqlCommand, int slotId)
        {
            sqlCommand.Parameters.AddWithValue("@BallId", item.Numbers[slotId].ToString());
            sqlCommand.Parameters.AddWithValue("@SlotId", (slotId+1).ToString());
            sqlCommand.Parameters.AddWithValue("@DrawingDate", item.DrawingDate.ToShortDateString());
            sqlCommand.Parameters.AddWithValue("@Game", item.Game.GetName(typeof(Game)));

            return sqlCommand;
        }

        public static SqlCommand MapDrawingToInsertDrawingDetails(this Drawing item, SqlCommand sqlCommand)
        {
            sqlCommand.Parameters.AddWithValue("@DrawingDate", item.DrawingDate.ToShortDateString());
            sqlCommand.Parameters.AddWithValue("@Game", item.Game.GetName(typeof(Game)));
            sqlCommand.Parameters.AddWithValue("@DrawingNumbers", item.KeyString);
            sqlCommand.Parameters.AddWithValue("@B1", item.Numbers[0].ToString());
            sqlCommand.Parameters.AddWithValue("@B2", item.Numbers[1].ToString());
            sqlCommand.Parameters.AddWithValue("@B3", item.Numbers[2].ToString());
            sqlCommand.Parameters.AddWithValue("@B4", item.Numbers[3].ToString());

            switch (item.Game)
            {
                case Game.Lotto:
                    sqlCommand.Parameters.AddWithValue("@B6", item.Numbers[5].ToString());
                    sqlCommand.Parameters.AddWithValue("@OB", DBNull.Value);
                    break;
                case Game.MegaMillion:
                    sqlCommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@OB", item.Numbers[5].ToString());
                    break;
                case Game.Powerball:
                    sqlCommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@OB", item.Numbers[5].ToString());
                    break;
                case Game.Hit5:
                    sqlCommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@OB", DBNull.Value);
                    break;
                case Game.Match4:
                default:
                    sqlCommand.Parameters.AddWithValue("@B5", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@OB", DBNull.Value);
                    break;
            }

            return sqlCommand;
        }

        public static Drawing MapGetValuesToDrawingItem(this object[] fields, Game game)
        {
            Drawing item = new Drawing()
            {
                DrawingDate = Convert.ToDateTime(fields[1].ToString()),
                Game = game
            };
            item.Numbers[0] = Convert.ToInt16(fields[4]);
            item.Numbers[1] = Convert.ToInt16(fields[5]);
            item.Numbers[2] = Convert.ToInt16(fields[6]);
            item.Numbers[3] = Convert.ToInt16(fields[7]);

            switch (game)
            {
                case Game.Lotto:
                    item.Numbers[4] = Convert.ToInt16(fields[8]);
                    item.Numbers[5] = Convert.ToInt16(fields[9]);
                    break;
                case Game.MegaMillion:
                    item.Numbers[4] = Convert.ToInt16(fields[8]);
                    item.Numbers[5] = Convert.ToInt16(fields[10]);
                    break;
                case Game.Powerball:
                    item.Numbers[4] = Convert.ToInt16(fields[8]);
                    item.Numbers[5] = Convert.ToInt16(fields[10]);
                    break;
                case Game.Hit5:
                    item.Numbers[4] = Convert.ToInt16(fields[8]);
                    break;
                case Game.Match4:
                default:
                    break;
            }


            return item;
        }
    }
}
