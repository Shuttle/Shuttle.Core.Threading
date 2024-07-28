using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading
{
    public interface IProcessorThreadPool : IDisposable
    {
        event EventHandler<ProcessorThreadCreatedEventArgs> ProcessorThreadCreated;

        void Stop();
        IProcessorThreadPool Start();
        Task<IProcessorThreadPool> StartAsync();
        IEnumerable<ProcessorThread> ProcessorThreads { get; }
        string Name { get; }
        IProcessorFactory ProcessorFactory { get; }
        ProcessorThreadOptions ThreadOptions { get; }
        int ThreadCount { get; }
    }
}