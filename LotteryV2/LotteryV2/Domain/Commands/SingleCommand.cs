using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryV2.Domain.Commands
{
    public class SingleCommand
    {
        private DrawingContext context;

        public void Execute( DrawingContext context)
        {
            this.context = context;

            this.Execute();

        }
        public void Execute()
        {
            this.context.ShouldExecuteSetHistoricalPeriods = true;
            foreach (var item in new CommandFactory().CreateCommands(this.context))
            {
                try
                {
                    if (item.ShouldExecute(this.context)) item.Execute(this.context);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
