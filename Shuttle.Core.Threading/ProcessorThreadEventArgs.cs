using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadEventArgs : EventArgs
    {
        public string Name { get; }
        public int ManagedThreadId { get; }

        public ProcessorThreadEventArgs(string name, int managedThreadId)
        {
            Name = Guard.AgainstNullOrEmptyString(name, nameof(name));
            ManagedThreadId = managedThreadId;
        }
    }
}