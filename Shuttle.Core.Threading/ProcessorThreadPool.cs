using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Threading;

public class ProcessorThreadPool : IProcessorThreadPool
{
    private readonly List<ProcessorThread> _processorThreads = new();
    private bool _disposed;
    private bool _started;

    public ProcessorThreadPool(string name, int threadCount, IProcessorFactory processorFactory, ProcessorThreadOptions processorThreadOptions)
    {
        if (threadCount < 1)
        {
            throw new ThreadCountZeroException();
        }

        Name = name;
        ProcessorFactory = Guard.AgainstNull(processorFactory);
        ThreadOptions = Guard.AgainstNull(processorThreadOptions);
        ThreadCount = threadCount;
    }

    public string Name { get; }
    public IProcessorFactory ProcessorFactory { get; }
    public ProcessorThreadOptions ThreadOptions { get; }
    public int ThreadCount { get; }

    public event EventHandler<ProcessorThreadCreatedEventArgs>? ProcessorThreadCreated;

    public async Task StopAsync()
    {
        foreach (var thread in _processorThreads)
        {
            await thread.StopAsync();
        }
    }

    public IEnumerable<ProcessorThread> ProcessorThreads => _processorThreads.AsReadOnly();

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    public async Task StartAsync()
    {
        if (_started)
        {
            return;
        }

        var i = 0;

        while (i++ < ThreadCount)
        {
            var processorThread = new ProcessorThread($"{Name} / {i}", ProcessorFactory.Create(), ThreadOptions);

            ProcessorThreadCreated?.Invoke(this, new(processorThread));

            _processorThreads.Add(processorThread);

            await processorThread.StartAsync();
        }

        _started = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (var thread in _processorThreads)
            {
                thread.Deactivate();
            }

            foreach (var thread in _processorThreads)
            {
                thread.StopAsync();
            }

            ProcessorFactory.TryDispose();
        }

        _disposed = true;
    }
}