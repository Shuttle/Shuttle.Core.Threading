using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ProcessorExceptionEventArgs : EventArgs
    {
        public string Name { get; }
        public int ManagedThreadId { get; }
        public string ProcessorTypeFullName { get; }
        public Exception Exception { get; }

        public ProcessorExceptionEventArgs(string name, int managedThreadId, string processorTypeFullName, Exception exception)
        {
            Name = Guard.AgainstNullOrEmptyString(name, nameof(name));
            ProcessorTypeFullName = Guard.AgainstNullOrEmptyString(processorTypeFullName, nameof(processorTypeFullName));
            Exception = Guard.AgainstNull(exception, nameof(exception));
            ManagedThreadId = managedThreadId;
        }
    }
}