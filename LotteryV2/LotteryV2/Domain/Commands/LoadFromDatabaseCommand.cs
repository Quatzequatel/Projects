using LotteryV2.Domain.Model;
using LotteryV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LotteryV2.Domain.Commands
{
    public class LoadFromDatabaseCommand : Command<DrawingContext>
    {
        //TBD Change to load from config.
        private string connectionString = "Data Source=OceanView;Initial Catalog=Lottery;Integrated Security=True";

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("LoadFromDatabaseCommand");
            Load(context);

        }

        private void Load(DrawingContext context)
        {
            List<Drawing> data = new List<Drawing>();
            Game gameType = DrawingContext.GameType;
            string queryStatement = $"SELECT * FROM [Lottery].[dbo].[Drawings] WHERE Game = '{gameType.ToString()}' Order by DrawingDate desc";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataReader dataReader = null;

                try
                {
                    con.Open();
                    SqlCommand command = new SqlCommand(queryStatement, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        object[] fields = new object[dataReader.FieldCount];
                        int fieldCount = dataReader.GetValues(fields);
                        data.Add(fields.MapGetValuesToDrawingItem(gameType));
                    }

                }

                finally
                {
                    if (dataReader != null) dataReader.Close();

                    if (con != null) con.Close();
                }
            }

            context.SetDrawings(data);
            Console.WriteLine($"LoadFromDatabaseCommand: {data.Count} drawings loaded.");
        }
    }
}
