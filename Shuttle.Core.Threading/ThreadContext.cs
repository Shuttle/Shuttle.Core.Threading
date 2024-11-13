using System;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading;

public class ProcessorThreadContext : IDisposable, IProcessorThreadContext
{
    public IState State { get; }
    public IServiceScope ServiceScope { get; }

    public ProcessorThreadContext(IState state, IServiceScope serviceScope)
    {
        State = Guard.AgainstNull(state);
        ServiceScope = Guard.AgainstNull(serviceScope);
    }

    public void Dispose()
    {
        ServiceScope.Dispose();
    }
}