﻿using System.Data.SqlClient;
using LotteryV2.Domain.Model;
using LotteryV2.Domain.Extensions;
using System;

namespace LotteryV2.Domain.Extensions
{
    public static class DatabaseExtensions
    {
        public static SqlCommand MapGetBallDrawingsinRangeParameters(this SqlCommand sqlcommand, string game, int slotId, DateTime startDate, DateTime endDate)
        {
            sqlcommand.Parameters.AddWithValue("@Game", game);
            sqlcommand.Parameters.AddWithValue("@SlotId", slotId.ToString());
            sqlcommand.Parameters.AddWithValue("@StartDate", startDate.ToShortDateString());
            sqlcommand.Parameters.AddWithValue("@EndDate", endDate.ToShortDateString());
            

            return sqlcommand;
        }

        public static SqlCommand MapResultToInsertBallTimesChosenInPeriodsDataSet(this GetTimesChosenInDateRangeItem item, SqlCommand sqlcommand)
        {
            sqlcommand.Parameters.AddWithValue("@StartDate", item.StartDate.ToShortDateString());
            sqlcommand.Parameters.AddWithValue("@Period", item.Period.ToString());
            sqlcommand.Parameters.AddWithValue("@BallId", item.BallId.ToString());
            sqlcommand.Parameters.AddWithValue("@SlotId", item.SlotId.ToString());
            sqlcommand.Parameters.AddWithValue("@Count", item.Count.ToString());
            sqlcommand.Parameters.AddWithValue("@Percent", item.Percent);
            sqlcommand.Parameters.AddWithValue("@Game", item.Game.GetName(typeof(Game)));

            return sqlcommand;
        }

        public static SqlCommand MapDrawingToInsertBallDrawingDetails(this Drawing item, SqlCommand sqlcommand, int slotId)
        {
            sqlcommand.Parameters.AddWithValue("@BallId", item.Numbers[slotId].ToString());
            sqlcommand.Parameters.AddWithValue("@SlotId", (slotId+1).ToString());
            sqlcommand.Parameters.AddWithValue("@DrawingDate", item.DrawingDate.ToShortDateString());
            sqlcommand.Parameters.AddWithValue("@Game", item.Game.GetName(typeof(Game)));

            return sqlcommand;
        }

        public static SqlCommand MapDrawingToInsertDrawingDetails(this Drawing item, SqlCommand sqlcommand)
        {
            sqlcommand.Parameters.AddWithValue("@DrawingDate", item.DrawingDate.ToShortDateString());
            sqlcommand.Parameters.AddWithValue("@Game", item.Game.GetName(typeof(Game)));
            sqlcommand.Parameters.AddWithValue("@DrawingNumbers", item.KeyString);
            sqlcommand.Parameters.AddWithValue("@B1", item.Numbers[0].ToString());
            sqlcommand.Parameters.AddWithValue("@B2", item.Numbers[1].ToString());
            sqlcommand.Parameters.AddWithValue("@B3", item.Numbers[2].ToString());
            sqlcommand.Parameters.AddWithValue("@B4", item.Numbers[3].ToString());

            switch (item.Game)
            {
                case Game.Lotto:
                    sqlcommand.Parameters.AddWithValue("@B6", item.Numbers[5].ToString());
                    sqlcommand.Parameters.AddWithValue("@OB", DBNull.Value);
                    break;
                case Game.MegaMillion:
                    sqlcommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    sqlcommand.Parameters.AddWithValue("@OB", item.Numbers[5].ToString());
                    break;
                case Game.Powerball:
                    sqlcommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    sqlcommand.Parameters.AddWithValue("@OB", item.Numbers[5].ToString());
                    break;
                case Game.Hit5:
                    sqlcommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    sqlcommand.Parameters.AddWithValue("@OB", DBNull.Value);
                    break;
                case Game.Match4:
                default:
                    sqlcommand.Parameters.AddWithValue("@B5", DBNull.Value);
                    sqlcommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    sqlcommand.Parameters.AddWithValue("@OB", DBNull.Value);
                    break;
            }

            return sqlcommand;
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
