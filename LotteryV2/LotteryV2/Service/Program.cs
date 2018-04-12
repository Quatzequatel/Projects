using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LotteryV2.Domain.Commands;
using System;

namespace LotteryV2
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandContext context = new CommandContext(Domain.Game.Lotto, new DateTime(2018, 4, 12));
            var commands = (new CommandFactory().CreateCommands(context));
            (new CommandExecutor<CommandContext>()).Execute(context, commands);
        }
    }
}
