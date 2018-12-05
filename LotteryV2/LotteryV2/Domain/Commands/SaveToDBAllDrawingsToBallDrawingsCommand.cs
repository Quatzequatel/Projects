using System;
using System.Data.SqlClient;
using LotteryV2.Domain.Extensions;

namespace LotteryV2.Domain.Commands
{
    class SaveToDBAllDrawingsToBallDrawingsCommand : Command<DrawingContext>
    {
        private string connectionString = "Data Source=OceanView;Initial Catalog=Lottery;Integrated Security=True";
        //EXEC @return_value = [dbo].[InsertBallDrawingDetails]
        //@BallId = 3,
        //@SlotId = 2,
        //@DrawingDate = '1/1/1900',
        //@Game = 'BitMe'
        private string SprocName = "[dbo].[InsertBallDrawingDetails]";

        //DrawingContext context;

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("SaveToDBAllDrawingsToBallDrawingsCommand");
            Save(context);
        }

        private void Save(DrawingContext context)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                bool isopen = false;
                foreach (var item in context.AllDrawings)
                {
                    if (!isopen)
                    {
                        isopen = true;
                        con.Open();
                    }

                    for (int i = 0; i < item.Numbers.Length; i++)
                    {
                        using (SqlCommand command = new SqlCommand(SprocName, con) { CommandType = System.Data.CommandType.StoredProcedure })
                        {
                            if (item.MapDrawingToInsertBallDrawingDetails(command, i).ExecuteNonQuery() < 0)
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
