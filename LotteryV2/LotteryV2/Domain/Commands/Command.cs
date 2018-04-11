using System;
using System.Collections.Generic;
using System.Linq;

namespace LotteryV2.Domain.Commands
{
    public interface ICommandFactory<T> where T : class
    {
        LinkedList<Command<T>> CreateCommands(T context);
        LinkedList<Command<T>> CreateAlternateCommands(T context);
    }


    public abstract class Command<T>
    {
        public bool IsRollbackEnabled { get; set; } = true;
        public virtual bool ShouldExecute(T context) => true;
        public virtual void Execute(T context, IEnumerable<string> additionalMetaData) { }
        public virtual void Execute(T context) { }
        public virtual void Rollback(T context) { }
        public static explicit operator Command<T>(Type v)
        {
            return (Command<T>)Activator.CreateInstance(v);
        }
    }
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
            typeof(LoadFromFile),
            typeof(SaveToJsonCommand),
            typeof(CreatNumbersModelCommand)
        };

        private static IEnumerable<Type> AlternateCommands()
        {
             throw new NotImplementedException();
        }
    }

    public class CommandExecutor<T>
    {
        public void Execute(T context, LinkedList<Command<T>> commands)
        {
            var command = commands.First;

            while (command != null)
            {
                try
                {
                    if (command.Value.ShouldExecute(context))
                        command.Value.Execute(context);
                    command = command.Next;

                }
                catch (Exception ex)
                {
                    HandleRollback(context, command);
                    throw;
                }
            }
        }

        private void HandleRollback(T context, LinkedListNode<Command<T>> command)
        {
            while (command != null && command.Value.IsRollbackEnabled)
            {
                command = command.Previous;
                command?.Value.Rollback(context);
            }
        }
    }
}
