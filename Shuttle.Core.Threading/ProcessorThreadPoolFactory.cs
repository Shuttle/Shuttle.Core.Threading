using System;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading;

public class ProcessorThreadPoolFactory : IProcessorThreadPoolFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ProcessorThreadPoolFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = Guard.AgainstNull(serviceScopeFactory);
    }

    public event EventHandler<ProcessorThreadPoolCreatedEventArgs>? ProcessorThreadPoolCreated;

    public IProcessorThreadPool Create(string name, int threadCount, IProcessorFactory processorFactory, ProcessorThreadOptions processorThreadOptions)
    {
        var result = new ProcessorThreadPool(name, threadCount, _serviceScopeFactory, processorFactory, processorThreadOptions);

        ProcessorThreadPoolCreated?.Invoke(this, new(result));

        return result;
    }
}