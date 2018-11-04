using System;
using System.Collections.Generic;

namespace LotteryV2.Domain.Commands
{
    public class NumberModelCommandFactory : ICommandFactory<AnalysisContext>
    {
        public LinkedList<Command<AnalysisContext>> CreateAlternateCommands(AnalysisContext context)
        {
            throw new NotImplementedException();
        }

        public LinkedList<Command<AnalysisContext>> CreateCommands(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}
