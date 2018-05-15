using System;
using System.Collections.Generic;

namespace LotteryV2.Domain.Commands
{
    public interface ICommandFactory<T> where T : class
    {
        LinkedList<Command<T>> CreateCommands(T context);
        LinkedList<Command<T>> CreateAlternateCommands(T context);
    }


    public abstract class Command<T> : IDisposable
    {
        public bool IsRollbackEnabled { get; set; } = true;
        public virtual bool ShouldExecute(T context) => true;
        public virtual void Execute(T context, IEnumerable<string> additionalMetaData) { }
        public virtual void Execute(T context) { }
        public virtual void Rollback(T context) { }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static explicit operator Command<T>(Type v)
        {
            return (Command<T>)Activator.CreateInstance(v);
        }
    }
}
