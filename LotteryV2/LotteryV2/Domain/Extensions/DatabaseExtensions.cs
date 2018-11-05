using System.Data.SqlClient;
using LotteryV2.Domain.Model;
using LotteryV2.Domain.Extensions;
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
