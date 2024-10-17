using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading;

public class ProcessorThreadEventArgs : EventArgs
{
    public ProcessorThreadEventArgs(string name, int managedThreadId)
    {
        Name = Guard.AgainstNullOrEmptyString(name);
        ManagedThreadId = managedThreadId;
    }

    public int ManagedThreadId { get; }
    public string Name { get; }
}