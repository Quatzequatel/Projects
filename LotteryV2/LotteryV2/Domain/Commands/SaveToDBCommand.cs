using System;
using System.Data.SqlClient;
using LotteryV2.Domain.Extensions;

namespace LotteryV2.Domain.Commands
{

    public class SaveToDBCommand : Command<DrawingContext>
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
//        private string InsertQuery = @"[dbo].[InsertDrawingDetails]
//([DrawingDate],[Game],[DrawingNumbers],[B1],[B2],[B3],[B4],[B5],[B6],[OB])
//VALUES(@DrawingDate,@Game,@DrawingNumbers,@B1,@B2,@B3,@B4,@B5,@B6,@OB)";
        private string SprocName = "[dbo].[InsertDrawingDetails]";
        private string SprocName2 = "[dbo].[InsertBallDrawingDetails]";

        public override bool ShouldExecute(DrawingContext context)
        {
            //TBD   need to do a check to see if the table needs to be updated.
            //TBD  The loop in Save() should start at new drawings.
            return true;
        }

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

                        for (int i = 0; i < item.Numbers.Length; i++)
                        {
                            using (SqlCommand command2 = new SqlCommand(SprocName2, con) { CommandType = System.Data.CommandType.StoredProcedure })
                            {
                                if (item.MapDrawingToInsertBallDrawingDetails(command2, i).ExecuteNonQuery() < 0)
                                {
                                    //throw new Exception("Error writing to DB."); 
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}