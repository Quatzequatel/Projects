using System;
using System.Collections.Generic;
using System.Linq;

namespace LotteryV2.Domain.Commands
{
    public class CommandFactory : ICommandFactory<DrawingContext>
    {
        public LinkedList<Command<DrawingContext>> CreateAlternateCommands(DrawingContext context)
        {
            return Resolve(AlternateCommands().ToList());
        }

        public LinkedList<Command<DrawingContext>> CreateCommands(DrawingContext context)
        {
            return Resolve(GetCommandTypes(context).ToList());
        }

        private static IEnumerable<Type> GetCommandTypes(DrawingContext context) =>
            context.IsAlternateContext ? AlternateCommands() : DefaultCommands;

        public LinkedList<Command<DrawingContext>> Resolve(List<Type> types)
        {
            return new LinkedList<Command<DrawingContext>>(types.Select(t => (Command<DrawingContext>)t));
        }

        private static IEnumerable<Type> DefaultCommands => new List<Type>
        {
            typeof(LoadFromFile), //load drawings from json file; if exists
            typeof(ScrapeFromWeb), //if no json file scrape from web and save.
            typeof(UpdateJsonFromWeb), // update json file by appending drawings.
            typeof(SaveJsonToFileCommand), //save to file.
            typeof(SaveBaseCSVCommand), //save to CSV file.
            typeof(SlotNumberAnalysis2CSVCommand),
            typeof(SlotNumberAnalysisRanged2CSVCommand)
        };

        private static IEnumerable<Type> AlternateCommands()
        {
             throw new NotImplementedException();
        }
    }
}
