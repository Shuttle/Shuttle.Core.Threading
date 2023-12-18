using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading.Tests;

public class MockProcessor : IProcessor
{
    private readonly TimeSpan _executionDuration;

    public int ExecutionCount { get; private set; }

    public MockProcessor(TimeSpan executionDuration)
    {
        _executionDuration = executionDuration;
    }

    public void Execute(CancellationToken cancellationToken)
    {
        ExecuteAsync(cancellationToken).GetAwaiter().GetResult();
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(_executionDuration, cancellationToken).ConfigureAwait(false);

        ExecutionCount++;
    }
}