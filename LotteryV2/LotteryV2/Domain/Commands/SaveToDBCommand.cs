using System;
using System.Data.SqlClient;
using LotteryV2.Domain.Extensions;

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
                        if (!isopen)
                        {
                            isopen = true;
                            con.Open();
                        }
                        if(item.MapDrawingToInsertDrawingDetails(command).ExecuteNonQuery() < 0)
                        {
                            //throw new Exception("Error writing to DB.");
                        }
                    }
                }
            }
        }
    }
}