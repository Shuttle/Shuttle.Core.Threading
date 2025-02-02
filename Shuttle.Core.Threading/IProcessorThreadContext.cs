using Microsoft.Extensions.DependencyInjection;

namespace Shuttle.Core.Threading;

public interface IProcessorThreadContext
{
    IState State { get; }
    IServiceScope ServiceScope { get; }
}