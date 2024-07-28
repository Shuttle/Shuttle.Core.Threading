using System;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadPoolFactory : IProcessorThreadPoolFactory
    {
        public event EventHandler<ProcessorThreadPoolCreatedEventArgs> ProcessorThreadPoolCreated;

        public IProcessorThreadPool Create(string name, int threadCount, IProcessorFactory processorFactory, ProcessorThreadOptions processorThreadOptions)
        {
            var result = new ProcessorThreadPool(name, threadCount, processorFactory, processorThreadOptions);

            ProcessorThreadPoolCreated?.Invoke(this, new ProcessorThreadPoolCreatedEventArgs(result));

            return result;
        }
    }
}