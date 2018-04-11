using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    internal class CreatNumbersModelCommand : Command<CommandContext>
    {

        public override void Execute(CommandContext context, IEnumerable<string> additionalMetaData)
        {
            Execute(context);
        }
        public override void Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }

    }

    public class NumberModelContext
    {
        public NumberModelContext(CommandContext parentContext)
        {
            NextDrawingDate = parentContext.NextDrawingDate;
            Drawings = parentContext.Drawings;
        }
        public readonly DateTime NextDrawingDate;
        public List<Drawing> Drawings { get; private set; }
    }

    public class NumberModelCommandFactory : ICommandFactory<NumberModelContext>
    {
        public LinkedList<Command<NumberModelContext>> CreateAlternateCommands(NumberModelContext context)
        {
            throw new NotImplementedException();
        }

        public LinkedList<Command<NumberModelContext>> CreateCommands(NumberModelContext context)
        {
            throw new NotImplementedException();
        }
    }
}
