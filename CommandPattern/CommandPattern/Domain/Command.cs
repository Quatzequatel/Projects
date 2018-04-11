using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CommandPattern.Domain
{
    //
    // Summary:
    //     Defines a command.
    public interface ICommand<in T> where T : class
    {
        //
        // Summary:
        //     Occurs when changes occur that affect whether or not the command should execute.
        event EventHandler CanExecuteChanged;

        //
        // Summary:
        //     Defines the method that determines whether the command can execute in its current
        //     state.
        //
        // Parameters:
        //   parameter:
        //     Data used by the command. If the command does not require data to be passed,
        //     this object can be set to null.
        //
        // Returns:
        //     true if this command can be executed; otherwise, false.
        bool CanExecute(T context);
        //
        // Summary:
        //     Defines the method to be called when the command is invoked.
        //
        // Parameters:
        //   parameter:
        //     Data used by the command. If the command does not require data to be passed,
        //     this object can be set to null.
        void Execute(T context);

        void Rollback(T context);
    }
    class Command : ICommand<CommandContext>
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(CommandContext context)
        {
            throw new NotImplementedException();
        }

        public void Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }

        public void Rollback(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }

    class CommandContext
    {

    }
}
