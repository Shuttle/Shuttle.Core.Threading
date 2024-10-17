using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading;

public class ProcessorThreadCreatedEventArgs : EventArgs
{
    public ProcessorThreadCreatedEventArgs(ProcessorThread processorThread)
    {
        ProcessorThread = Guard.AgainstNull(processorThread);
    }

    public ProcessorThread ProcessorThread { get; }
}