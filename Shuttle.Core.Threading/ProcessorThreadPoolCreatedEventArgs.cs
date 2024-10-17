using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading;

public class ProcessorThreadPoolCreatedEventArgs : EventArgs
{
    public ProcessorThreadPoolCreatedEventArgs(IProcessorThreadPool processorThreadPool)
    {
        ProcessorThreadPool = Guard.AgainstNull(processorThreadPool);
    }

    public IProcessorThreadPool ProcessorThreadPool { get; }
}