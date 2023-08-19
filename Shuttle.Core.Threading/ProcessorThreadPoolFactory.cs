using System;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadPoolFactory : IProcessorThreadPoolFactory
    {
        public event EventHandler<ProcessorThreadPoolCreatedEventArgs> ProcessorThreadPoolCreated = delegate
        {
        };

        public IProcessorThreadPool Create(string name, int threadCount, IProcessorFactory processorFactory, ProcessorThreadOptions processorThreadOptions)
        {
            var result = new ProcessorThreadPool(name, threadCount, processorFactory, processorThreadOptions);

            ProcessorThreadPoolCreated.Invoke(this, new ProcessorThreadPoolCreatedEventArgs(name, threadCount, processorFactory));

            return result;
        }
    }
}