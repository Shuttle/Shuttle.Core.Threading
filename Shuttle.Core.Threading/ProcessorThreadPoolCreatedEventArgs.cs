using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadPoolCreatedEventArgs : EventArgs
    {
        public string Name { get; }
        public int ThreadCount { get; }
        public IProcessorFactory ProcessorFactory { get; }

        public ProcessorThreadPoolCreatedEventArgs(string name, int threadCount, IProcessorFactory processorFactory)
        {
            Name = Guard.AgainstNullOrEmptyString(name, nameof(name));
            ThreadCount = threadCount;
            ProcessorFactory = Guard.AgainstNull(processorFactory, nameof(processorFactory));
        }
    }
}