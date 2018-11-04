using System;
using System.Data.SqlClient;
using LotteryV2.Domain.Model;

namespace LotteryV2.Domain.Commands
{

    public class SaveToDBCommand : Command<DrawingContext>
    {
        private string connectionString = "Data Source=OceanView;Initial Catalog=Lottery;Integrated Security=True";
//        private string InsertQuery = @"[dbo].[InsertDrawingDetails]
//([DrawingDate],[Game],[DrawingNumbers],[B1],[B2],[B3],[B4],[B5],[B6],[OB])
//VALUES(@DrawingDate,@Game,@DrawingNumbers,@B1,@B2,@B3,@B4,@B5,@B6,@OB)";
        private string SprocName = "[dbo].[InsertDrawingDetails]";

        //DrawingContext context;

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("SaveToDBCommand");
            Save(context);
        }

        private void Save(DrawingContext context)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                bool isopen = false;
                foreach (var item in context.AllDrawings)
                {
                    using (SqlCommand command = new SqlCommand(SprocName, con) {CommandType= System.Data.CommandType.StoredProcedure})
                    {

                        command.Parameters.AddWithValue("@DrawingDate", item.DrawingDate.ToShortDateString());
                        command.Parameters.AddWithValue("@Game", item.Game.GetName(typeof(Game)));
                        command.Parameters.AddWithValue("@DrawingNumbers", item.KeyString);
                        command.Parameters.AddWithValue("@B1", item.Numbers[0].ToString());
                        command.Parameters.AddWithValue("@B2", item.Numbers[1].ToString());
                        command.Parameters.AddWithValue("@B3", item.Numbers[2].ToString());
                        command.Parameters.AddWithValue("@B4", item.Numbers[3].ToString());
                        if (item.Game != Game.Match4)
                        {
                            command.Parameters.AddWithValue("@B5", item.Numbers[4]);
                            if (item.Game != Game.MegaMillion && item.Game != Game.Powerball)
                            {
                                if (item.Game == Game.Hit5)
                                {
                                    command.Parameters.AddWithValue("@B6", DBNull.Value);
                                }
                                else //lotto
                                {
                                    command.Parameters.AddWithValue("@B6", item.Numbers[5].ToString());
                                }
                                command.Parameters.AddWithValue("@OB", DBNull.Value);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@B6", DBNull.Value);
                                command.Parameters.AddWithValue("@OB", item.Numbers[5].ToString());
                            }
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@B5", DBNull.Value);
                            command.Parameters.AddWithValue("@B6", DBNull.Value);
                            command.Parameters.AddWithValue("@OB", DBNull.Value);
                        }
                        if (!isopen)
                        {
                            isopen = true;
                            con.Open();
                        }
                        if(command.ExecuteNonQuery() < 0)
                        {
                            //throw new Exception("Error writing to DB.");
                        }
                    }
                }
            }
        }
    }
}