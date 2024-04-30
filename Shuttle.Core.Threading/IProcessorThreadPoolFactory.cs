using System;

namespace Shuttle.Core.Threading
{
    public interface IProcessorThreadPoolFactory
    {
        event EventHandler<ProcessorThreadPoolCreatedEventArgs> ProcessorThreadPoolCreated;

        IProcessorThreadPool Create(string name, int threadCount, IProcessorFactory processorFactory, ProcessorThreadOptions processorThreadOptions);
    }
}