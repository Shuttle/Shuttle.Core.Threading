using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadExceptionEventArgs : EventArgs
    {
        public string Name { get; }
        public int ManagedThreadId { get; }
        public Exception Exception { get; }

        public ProcessorThreadExceptionEventArgs(string name, int managedThreadId, Exception exception)
        {
            Name = Guard.AgainstNullOrEmptyString(name, nameof(name));
            ManagedThreadId = managedThreadId;
            Exception = Guard.AgainstNull(exception, nameof(exception));
        }
    }
}