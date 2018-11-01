using System;
using System.Data.SqlClient;

namespace LotteryV2.Domain.Commands
{
    public class SaveToDBCommand : Command<DrawingContext>
    {
        private string connectionString = "Data Source=OceanView;Initial Catalog=Lottery;Integrated Security=True";
        public SaveToDBCommand()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {

            }
        }
    }
}
