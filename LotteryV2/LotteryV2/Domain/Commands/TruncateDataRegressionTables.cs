using System;
using System.Data.SqlClient;
using System.Linq;
using LotteryV2.Domain.Extensions;
using System.Collections.Generic;
using LotteryV2.Domain.Model;

namespace LotteryV2.Domain.Commands
{
    class TruncateDataRegressionTables: Command<DrawingContext>
    {
        private DrawingContext Context;
        private bool IsOpen = false;
        private SqlConnection Connection;

        public override bool ShouldExecute(DrawingContext context)
        {
            Context = context;
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            TruncateBallTimesChosenInPeriodsDataSet();
            TruncateSlopeInterceptDetails();
        }

        private void TruncateBallTimesChosenInPeriodsDataSet()
        {
            Connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString);
            OpenConnection();
            new SqlCommand("truncate table [dbo].[BallTimesChosenInPeriodsDataSet]", Connection)
            { CommandType = System.Data.CommandType.Text }
            .ExecuteNonQuery();
            CloseConnection();
        }

        private void TruncateSlopeInterceptDetails()
        {
            Connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString);
            OpenConnection();
            new SqlCommand("truncate table [dbo].[SlopeInterceptDetails]", Connection)
            { CommandType = System.Data.CommandType.Text }
            .ExecuteNonQuery();
            CloseConnection();
        }

        private void OpenConnection()
        {
            if (!IsOpen && Connection != null)
            {
                IsOpen = true;
                Connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (Connection != null)
            {
                Connection.Close();
                IsOpen = false;
            }
        }
    }
}
