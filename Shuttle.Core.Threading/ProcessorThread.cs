using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Threading
{
    public class ProcessorThread
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ProcessorThreadOptions _processorThreadOptions;
        private ProcessorThreadEventArgs _eventArgs;

        private bool _started;
        private bool _sync;
        private Thread _thread;

        private readonly Dictionary<string, object> _state = new Dictionary<string, object>();

        public ProcessorThread(string name, IProcessor processor, ProcessorThreadOptions processorThreadOptions)
        {
            Name = Guard.AgainstNull(name, nameof(name));
            Processor = Guard.AgainstNull(processor, nameof(processor));
            _processorThreadOptions = Guard.AgainstNull(processorThreadOptions, nameof(processorThreadOptions));
            CancellationToken = _cancellationTokenSource.Token;
        }

        public CancellationToken CancellationToken { get; }

        public string Name { get; }
        public IProcessor Processor { get; }

        internal void Deactivate()
        {
            _cancellationTokenSource.Cancel();
        }

        public event EventHandler<ProcessorThreadExceptionEventArgs> ProcessorException;
        public event EventHandler<ProcessorThreadEventArgs> ProcessorExecuting;
        public event EventHandler<ProcessorThreadEventArgs> ProcessorThreadActive;
        public event EventHandler<ProcessorThreadEventArgs> ProcessorThreadOperationCanceled;
        public event EventHandler<ProcessorThreadEventArgs> ProcessorThreadStarting;
        public event EventHandler<ProcessorThreadStoppedEventArgs> ProcessorThreadStopped;
        public event EventHandler<ProcessorThreadEventArgs> ProcessorThreadStopping;

        public void Start()
        {
            Start(true);
        }

        public void Start(bool sync)
        {
            if (_started)
            {
                return;
            }

            _sync = sync;

            _thread = new Thread(Work) { Name = Name };

            _thread.TrySetApartmentState(ApartmentState.MTA);

            _thread.IsBackground = _processorThreadOptions.IsBackground;
            _thread.Priority = _processorThreadOptions.Priority;

            _eventArgs = new ProcessorThreadEventArgs(Name, _thread.ManagedThreadId);

            _thread.Start();

            while (!_thread.IsAlive && !CancellationToken.IsCancellationRequested)
            {
            }

            if (!CancellationToken.IsCancellationRequested)
            {
                ProcessorThreadActive?.Invoke(this, _eventArgs);
            }

            _started = true;
        }

        public async Task StartAsync()
        {
            Start(false);

            await Task.CompletedTask;
        }

        public void Stop()
        {
            if (!_started)
            {
                throw new InvalidOperationException(Resources.ProcessorThreadNotStartedException);
            }

            ProcessorThreadStopping?.Invoke(this, _eventArgs);

            _cancellationTokenSource.Cancel();

            Processor.TryDispose();

            var aborted = false;
            var joinTimeout = _processorThreadOptions.JoinTimeout;

            if (joinTimeout.TotalSeconds < 1)
            {
                joinTimeout = TimeSpan.FromSeconds(1);
            }

            if (_thread.IsAlive && !_thread.Join(joinTimeout))
            {
                try
                {
                    _thread.Abort();
                }
                catch (ThreadAbortException)
                {
                    aborted = true;
                }
            }

            ProcessorThreadStopped?.Invoke(this, new ProcessorThreadStoppedEventArgs(_eventArgs.Name, _eventArgs.ManagedThreadId, aborted));
        }

        private async void Work()
        {
            ProcessorThreadStarting?.Invoke(this, _eventArgs);

            while (!CancellationToken.IsCancellationRequested)
            {
                ProcessorExecuting?.Invoke(this, _eventArgs);

                try
                {
                    if (_sync)
                    {
                        Processor.Execute(CancellationToken);
                    }
                    else
                    {
                        await Processor.ExecuteAsync(CancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    ProcessorThreadOperationCanceled?.Invoke(this, _eventArgs);
                }
                catch (Exception ex)
                {
                    ProcessorException?.Invoke(this, new ProcessorThreadExceptionEventArgs(_eventArgs.Name, _eventArgs.ManagedThreadId, ex));
                }
            }
        }

        public void SetState(string key, object value)
        {
            Guard.AgainstNullOrEmptyString(key, nameof(key));

            _state[key] = value;
        }

        public object GetState(string key)
        {
            Guard.AgainstNullOrEmptyString(key, nameof(key));

            return _state.ContainsKey(key) ? _state[key] : null;
        }
    }
}