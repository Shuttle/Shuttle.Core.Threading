using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading;

public class ProcessorThreadExceptionEventArgs : EventArgs
{
    public ProcessorThreadExceptionEventArgs(string name, int managedThreadId, Exception exception)
    {
        Name = Guard.AgainstNullOrEmptyString(name);
        ManagedThreadId = managedThreadId;
        Exception = Guard.AgainstNull(exception);
    }

    public Exception Exception { get; }
    public int ManagedThreadId { get; }
    public string Name { get; }
}