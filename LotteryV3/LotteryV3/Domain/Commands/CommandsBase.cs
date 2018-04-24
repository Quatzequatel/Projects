using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV3.Domain.Commands
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

    public class CommandExecutor<T>
    {
        public void Execute(T context, LinkedList<Command<T>> commands)
        {
            var command = commands.First;

            while (command != null)
            {
                //try
                //{
                if (command.Value.ShouldExecute(context))
                    command.Value.Execute(context);
                command = command.Next;

                //}
                //catch (Exception ex)
                //{
                //    HandleRollback(context, command);
                //    throw;
                //}
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

