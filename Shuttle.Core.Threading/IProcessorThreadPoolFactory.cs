using System;

namespace Shuttle.Core.Threading;

public interface IProcessorThreadPoolFactory
{
    IProcessorThreadPool Create(string name, int threadCount, IProcessorFactory processorFactory, ProcessorThreadOptions processorThreadOptions);
    event EventHandler<ProcessorThreadPoolCreatedEventArgs> ProcessorThreadPoolCreated;
}