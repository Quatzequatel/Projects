using System;
using System.Collections.Generic;

namespace LotteryV2.Domain.Commands
{
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
