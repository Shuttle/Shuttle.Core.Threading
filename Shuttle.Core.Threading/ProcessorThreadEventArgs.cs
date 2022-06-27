using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadEventArgs : EventArgs
    {
        public string Name { get; }
        public int ManagedThreadId { get; }
        public string ProcessorTypeFullName { get; }

        public ProcessorThreadEventArgs(string name, int managedThreadId, string processorTypeFullName)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));
            Guard.AgainstNullOrEmptyString(processorTypeFullName, nameof(processorTypeFullName));

            Name = name;
            ManagedThreadId = managedThreadId;
            ProcessorTypeFullName = processorTypeFullName;
        }
    }
}