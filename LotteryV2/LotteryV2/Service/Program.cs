using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LotteryV2.Domain.Commands;
using System;
using LotteryV2.Domain;
using System.Collections.Generic;

namespace LotteryV2
{
    class Program
    {
        static void Main(string[] args)
        {
            //DrawingContext context = new DrawingContext(Domain.Game.Lotto, new DateTime(2018, 4, 12));
            //var commands = (new CommandFactory().CreateCommands(context));
            //(new CommandExecutor<DrawingContext>()).Execute(context, commands);

            //List<Game> Games = new List<Game>() { Game.MegaMillion, Game.Powerball};
            List<Game> Games = new List<Game>() { Game.Match4 };
            //List<Game> Games = new List<Game>() { Game.Lotto, Game.MegaMillion, Game.Powerball };
            //List<Game> Games = new List<Game>() { Game.Match4, Game.Hit5, Game.Lotto, Game.MegaMillion, Game.Powerball };
            //List<Game> Games = new List<Game>() { Game.Hit5, Game.Lotto };

            foreach (var game in Games)
            {
                DrawingContext context = new DrawingContext(game)
                {
                    SampleSize = 1000,
                    CommandsType = CommandsType.GenerateData,
                    IsCompleteDownload = false,
                    SkipScrapeFromWeb = false,
                    ShouldExecuteSetHistoricalPeriods = true
                };

                foreach (var item in new CommandFactory().CreateCommands(context))
                {
                    try
                    {
                        if (item.ShouldExecute(context)) item.Execute(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                        throw;
                    }
                }
            }

        }
    }
}
