using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading;

public interface IProcessorThreadPool : IDisposable
{
    event EventHandler<ProcessorThreadCreatedEventArgs> ProcessorThreadCreated;

    string Name { get; }
    IProcessorFactory ProcessorFactory { get; }
    IEnumerable<ProcessorThread> ProcessorThreads { get; }
    int ThreadCount { get; }
    ProcessorThreadOptions ThreadOptions { get; }
    Task StartAsync();
    Task StopAsync();
}