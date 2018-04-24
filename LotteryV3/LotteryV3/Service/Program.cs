using System;
using LotteryV3.Domain;
using LotteryV3.Domain.Commands;
using LotteryV3.Domain.Entities;


namespace LotteryV3
{
    class Program
    {
        static void Main(string[] args)
        {
            DrawingContext context = new DrawingContext(GameType.Lotto, new DateTime(2018, 4, 12), new DateTime(1984, 7, 21));
            var commands = (new CommandFactory().CreateCommands(context));
            (new CommandExecutor<DrawingContext>()).Execute(context, commands);
        }
    }
}
