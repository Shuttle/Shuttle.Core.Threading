using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Threading;

public class ProcessorThread
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ProcessorThreadOptions _processorThreadOptions;

    private readonly ProcessorThreadEventArgs _eventArgs;

    private bool _started;
    private readonly Thread _thread;

    public ProcessorThread(string name, IServiceScopeFactory serviceScopeFactory, IProcessor processor, ProcessorThreadOptions processorThreadOptions)
    {
        _serviceScopeFactory = Guard.AgainstNull(serviceScopeFactory);
        Name = Guard.AgainstNull(name);
        Processor = Guard.AgainstNull(processor);
        _processorThreadOptions = Guard.AgainstNull(processorThreadOptions);
        CancellationToken = _cancellationTokenSource.Token;

        _thread = new(Work) { Name = Name };

        _thread.TrySetApartmentState(ApartmentState.MTA);

        _thread.IsBackground = _processorThreadOptions.IsBackground;
        _thread.Priority = _processorThreadOptions.Priority;

        _eventArgs = new(Name, _thread.ManagedThreadId);

        State.Add("Name", Name);
        State.Add("ManagedThreadId", _thread.ManagedThreadId);
    }

    public CancellationToken CancellationToken { get; }

    public string Name { get; }
    public IProcessor Processor { get; }
    public int ManagedThreadId => _thread.ManagedThreadId;

    internal void Deactivate()
    {
        _cancellationTokenSource.Cancel();
    }

    public IState State { get; } = new State();

    public event EventHandler<ProcessorThreadExceptionEventArgs>? ProcessorException;
    public event EventHandler<ProcessorThreadEventArgs>? ProcessorExecuting;
    public event EventHandler<ProcessorThreadEventArgs>? ProcessorThreadActive;
    public event EventHandler<ProcessorThreadEventArgs>? ProcessorThreadOperationCanceled;
    public event EventHandler<ProcessorThreadEventArgs>? ProcessorThreadStarting;
    public event EventHandler<ProcessorThreadEventArgs>? ProcessorThreadStopped;
    public event EventHandler<ProcessorThreadEventArgs>? ProcessorThreadStopping;

    public async Task StartAsync()
    {
        if (_started)
        {
            return;
        }

        _thread.Start();

        while (!_thread.IsAlive && !CancellationToken.IsCancellationRequested)
        {
        }

        if (!CancellationToken.IsCancellationRequested)
        {
            ProcessorThreadActive?.Invoke(this, _eventArgs);
        }

        _started = true;

        await Task.CompletedTask;
    }

    public Task StopAsync()
    {
        if (!_started)
        {
            throw new InvalidOperationException(Resources.ProcessorThreadNotStartedException);
        }

        _cancellationTokenSource.Cancel();

        Processor.TryDispose();

        var joinTimeout = _processorThreadOptions.JoinTimeout;

        if (joinTimeout.TotalSeconds < 1)
        {
            joinTimeout = TimeSpan.FromSeconds(1);
        }

        if (_thread is { IsAlive: true } && !_thread.Join(joinTimeout))
        {
            throw new ApplicationException(Resources.ProcessorThreadJoinTimeoutException);
        }

        ProcessorThreadStopped?.Invoke(this, new(_eventArgs.Name, _eventArgs.ManagedThreadId));

        return Task.CompletedTask;
    }

    private async void Work()
    {
        ProcessorThreadStarting?.Invoke(this, _eventArgs);

        while (!CancellationToken.IsCancellationRequested)
        {
            ProcessorExecuting?.Invoke(this, _eventArgs);

            try
            {
                using (var context = new ProcessorThreadContext(State, _serviceScopeFactory.CreateScope()))
                {
                    await Processor.ExecuteAsync(context, CancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                ProcessorThreadOperationCanceled?.Invoke(this, _eventArgs);
            }
            catch (Exception ex)
            {
                ProcessorException?.Invoke(this, new(_eventArgs.Name, _eventArgs.ManagedThreadId, ex));
            }
        }

        ProcessorThreadStopping?.Invoke(this, _eventArgs);
    }
}