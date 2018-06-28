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

            List<Game> Games = new List<Game>() { Game.Lotto};
            //List<Game> Games = new List<Game>() { Game.Match4, Game.Hit5 };
            //List<Game> Games = new List<Game>() {  Game.Lotto, Game.MegaMillion, Game.Powerball };
            //List<Game> Games = new List<Game>() { Game.Match4, Game.Hit5, Game.Lotto, Game.MegaMillion, Game.Powerball };
            //List<Game> Games = new List<Game>() { Game.Match4, Game.Hit5, Game.Lotto };

            foreach (var game in Games)
            {
                SingleCommand justDoIt = new SingleCommand();
                DrawingContext context = new DrawingContext(game) { SampleSize = 1000 };
                justDoIt.Execute(context);
            }

        }
    }
}
