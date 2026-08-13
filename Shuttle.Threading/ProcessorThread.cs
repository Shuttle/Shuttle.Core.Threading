using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shuttle.Contract;
using Shuttle.Reflection;

namespace Shuttle.Threading;

public class ProcessorThread(string serviceKey, IServiceScopeFactory serviceScopeFactory, ThreadingOptions threadingOptions, IProcessorIdleStrategy processorIdleStrategy, ILogger<ProcessorThread>? logger = null)
    : IProcessorThread
{
    private readonly IProcessorIdleStrategy _processorIdleStrategy = Guard.AgainstNull(processorIdleStrategy);
    private readonly IServiceScopeFactory _serviceScopeFactory = Guard.AgainstNull(serviceScopeFactory);
    private readonly ThreadingOptions _threadingOptions = Guard.AgainstNull(threadingOptions);
    private readonly ILogger<ProcessorThread> _logger = logger ?? NullLogger<ProcessorThread>.Instance;
    private CancellationToken _cancellationToken;
    private Task? _executionTask;
    private bool _started;
    public int ManagedThreadId { get; private set; }

    public string ServiceKey { get; } = Guard.AgainstNull(serviceKey);

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_started)
        {
            return;
        }

        _cancellationToken = cancellationToken;
        _executionTask = Task.Factory.StartNew(WorkAsync, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

        if (!cancellationToken.IsCancellationRequested)
        {
            LogMessage.ProcessorThreadActive(_logger, ServiceKey, Environment.CurrentManagedThreadId);

            await _threadingOptions.ProcessorThreadActive.InvokeAsync(new(this, Environment.CurrentManagedThreadId), cancellationToken);
        }

        _started = true;
    }

    public async Task StopAsync()
    {
        if (!_started)
        {
            throw new InvalidOperationException(Resources.ProcessorThreadNotStartedException);
        }

        if (_executionTask != null)
        {
            var joinTimeout = _threadingOptions.JoinTimeout;

            if (joinTimeout.TotalSeconds < 1)
            {
                joinTimeout = TimeSpan.FromSeconds(1);
            }

            try
            {
                await _executionTask.WaitAsync(joinTimeout, CancellationToken.None);
            }
            catch (TimeoutException)
            {
                throw new ApplicationException(Resources.ProcessorThreadJoinTimeoutException);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation occurs
            }
        }

        LogMessage.ProcessorThreadStopped(_logger, ServiceKey, Environment.CurrentManagedThreadId);

        await _threadingOptions.ProcessorThreadStopped.InvokeAsync(new(this, Environment.CurrentManagedThreadId), CancellationToken.None);
    }

    private async Task WorkAsync()
    {
        ManagedThreadId = Environment.CurrentManagedThreadId;

        var eventArgs = new ProcessorThreadEventArgs(this, ManagedThreadId);

        LogMessage.ProcessorThreadStarting(_logger, ServiceKey, ManagedThreadId);

        await _threadingOptions.ProcessorThreadStarting.InvokeAsync(eventArgs, _cancellationToken);

        while (!_cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var accessor = scope.ServiceProvider.GetRequiredService<ProcessorContextAccessor>();

                accessor.Context = new ProcessorContext(ServiceKey, ManagedThreadId);

                var processor = scope.ServiceProvider.GetRequiredKeyedService<IProcessor>(ServiceKey);

                LogMessage.ProcessorExecuting(_logger, ServiceKey, processor.GetType().FullName ?? processor.GetType().Name, ManagedThreadId);

                await _threadingOptions.ProcessorExecuting.InvokeAsync(new(ServiceKey, ManagedThreadId, processor), _cancellationToken);

                var workPerformed = await processor.ExecuteAsync(_cancellationToken);

                await _processorIdleStrategy.SignalAsync(ServiceKey, workPerformed, _cancellationToken);

                LogMessage.ProcessorExecuted(_logger, ServiceKey, processor.GetType().FullName ?? processor.GetType().Name, ManagedThreadId);

                await _threadingOptions.ProcessorExecuted.InvokeAsync(new(ServiceKey, ManagedThreadId, processor, workPerformed), _cancellationToken);
                await processor.TryDisposeAsync();
            }
            catch (OperationCanceledException) when (_cancellationToken.IsCancellationRequested)
            {
                LogMessage.ProcessorThreadOperationCanceled(_logger, ServiceKey, ManagedThreadId);

                await RaiseAsync(() => _threadingOptions.ProcessorThreadOperationCanceled.InvokeAsync(eventArgs, _cancellationToken));

                break;
            }
            catch (Exception ex)
            {
                // An `OperationCanceledException` that is raised while no cancellation has been requested (such as a
                // `TaskCanceledException` from an `HttpClient` timeout within a processor) is an ordinary failure and
                // should never end the work loop since the thread is not re-created.
                LogMessage.ProcessorException(_logger, ServiceKey, ManagedThreadId, ex);

                await RaiseAsync(() => _threadingOptions.ProcessorException.InvokeAsync(new(this, ManagedThreadId, ex), _cancellationToken));
            }
        }

        LogMessage.ProcessorThreadStopping(_logger, ServiceKey, ManagedThreadId);

        await RaiseAsync(() => _threadingOptions.ProcessorThreadStopping.InvokeAsync(eventArgs, _cancellationToken));
    }

    /// <summary>
    ///     Raises an event that is invoked from within, or after, an exception handler.  A subscriber that throws may
    ///     not be allowed to escape `WorkAsync` since the processor thread would then be orphaned; neither the thread
    ///     pool nor the thread itself re-creates the work loop.
    /// </summary>
    private async Task RaiseAsync(Func<Task> raise)
    {
        try
        {
            await raise().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogMessage.ProcessorException(_logger, ServiceKey, ManagedThreadId, ex);
        }
    }
}