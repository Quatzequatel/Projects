using System.Data.SqlClient;
using LotteryV2.Domain.Model;
using System;

namespace LotteryV2.Domain.Extensions
{
    public static class DatabaseExtensions
    {
        public static SqlCommand MapDrawingToInsertDrawingDetails(this Drawing item, SqlCommand sqlcommand)
        {
            sqlcommand.Parameters.AddWithValue("@DrawingDate", item.DrawingDate.ToShortDateString());
            sqlcommand.Parameters.AddWithValue("@Game", item.Game.GetName(typeof(Game)));
            sqlcommand.Parameters.AddWithValue("@DrawingNumbers", item.KeyString);
            sqlcommand.Parameters.AddWithValue("@B1", item.Numbers[0].ToString());
            sqlcommand.Parameters.AddWithValue("@B2", item.Numbers[1].ToString());
            sqlcommand.Parameters.AddWithValue("@B3", item.Numbers[2].ToString());
            sqlcommand.Parameters.AddWithValue("@B4", item.Numbers[3].ToString());
            if (item.Game != Game.Match4)
            {
                sqlcommand.Parameters.AddWithValue("@B5", item.Numbers[4]);
                if (item.Game != Game.MegaMillion && item.Game != Game.Powerball)
                {
                    if (item.Game == Game.Hit5)
                    {
                        sqlcommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    }
                    else //lotto
                    {
                        sqlcommand.Parameters.AddWithValue("@B6", item.Numbers[5].ToString());
                    }
                    sqlcommand.Parameters.AddWithValue("@OB", DBNull.Value);
                }
                else
                {
                    sqlcommand.Parameters.AddWithValue("@B6", DBNull.Value);
                    sqlcommand.Parameters.AddWithValue("@OB", item.Numbers[5].ToString());
                }
            }
            else
            {
                sqlcommand.Parameters.AddWithValue("@B5", DBNull.Value);
                sqlcommand.Parameters.AddWithValue("@B6", DBNull.Value);
                sqlcommand.Parameters.AddWithValue("@OB", DBNull.Value);
            }

            return sqlcommand;
        }
    }
}
