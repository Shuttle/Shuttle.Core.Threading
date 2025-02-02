using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading.Tests;

public class MockProcessor : IProcessor
{
    private readonly TimeSpan _executionDuration;

    public MockProcessor(TimeSpan executionDuration)
    {
        _executionDuration = executionDuration;
    }

    public int ExecutionCount { get; private set; }

    public async Task ExecuteAsync(IProcessorThreadContext context, CancellationToken cancellationToken)
    {
        await Task.Delay(_executionDuration, cancellationToken).ConfigureAwait(false);
        ExecutionCount++;
    }
}