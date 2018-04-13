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
            DrawingContext context = new DrawingContext(Domain.Game.Lotto, new DateTime(2018, 4, 12));
            var commands = (new CommandFactory().CreateCommands(context));
            (new CommandExecutor<DrawingContext>()).Execute(context, commands);
        }
    }
}
