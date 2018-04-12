using System;
using System.Collections.Generic;
using System.Linq;

namespace LotteryV2.Domain.Commands
{
    public class CommandFactory : ICommandFactory<CommandContext>
    {
        public LinkedList<Command<CommandContext>> CreateAlternateCommands(CommandContext context)
        {
            return Resolve(AlternateCommands().ToList());
        }

        public LinkedList<Command<CommandContext>> CreateCommands(CommandContext context)
        {
            return Resolve(GetCommandTypes(context).ToList());
        }

        private static IEnumerable<Type> GetCommandTypes(CommandContext context) =>
            context.IsAlternateContext ? AlternateCommands() : DefaultCommands;

        public LinkedList<Command<CommandContext>> Resolve(List<Type> types)
        {
            return new LinkedList<Command<CommandContext>>(types.Select(t => (Command<CommandContext>)t));
        }

        private static IEnumerable<Type> DefaultCommands => new List<Type>
        {
            typeof(LoadFromFile), //load drawings from json file; if exists
            typeof(ScrapeFromWeb), //if no json file scrape from web and save.
            typeof(UpdateJsonFromWeb), // update json file by appending drawings.
            typeof(SaveJsonToFileCommand), //save to file.
            //typeof(CreatNumbersModelCommand)
        };

        private static IEnumerable<Type> AlternateCommands()
        {
             throw new NotImplementedException();
        }
    }
}
