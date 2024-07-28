using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadPoolCreatedEventArgs : EventArgs
    {
        public IProcessorThreadPool ProcessorThreadPool { get;}

        public ProcessorThreadPoolCreatedEventArgs(IProcessorThreadPool processorThreadPool)
        {
            ProcessorThreadPool = Guard.AgainstNull(processorThreadPool, nameof(processorThreadPool));
        }
    }
}