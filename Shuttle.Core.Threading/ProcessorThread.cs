using System;
using System.Threading;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Threading
{
    public class ProcessorThread
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly string _name;
        private readonly IProcessor _processor;
        private ProcessorThreadEventArgs _eventArgs;

        private bool _started;
        private Thread _thread;

        public ProcessorThread(string name, IProcessor processor)
        {
            Guard.AgainstNull(processor, nameof(processor));

            _name = name;
            _processor = processor;

            CancellationToken = _cancellationTokenSource.Token;
        }

        public CancellationToken CancellationToken { get; }

        public event EventHandler<ProcessorThreadEventArgs> ProcessorThreadStarting = delegate
        {
        };

        public event EventHandler<ProcessorThreadEventArgs> ProcessorThreadActive = delegate
        {
        };

        public event EventHandler<ProcessorThreadEventArgs> ProcessorThreadStopping = delegate
        {
        };

        public event EventHandler<ProcessorThreadEventArgs> ProcessorThreadStopped = delegate
        {
        };

        public event EventHandler<ProcessorThreadEventArgs> ProcessorExecuting = delegate
        {
        };

        public void Start()
        {
            if (_started)
            {
                return;
            }

            _thread = new Thread(Work) { Name = _name };

            _thread.TrySetApartmentState(ApartmentState.MTA);

            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Normal;

            _thread.Start();

            _eventArgs = new ProcessorThreadEventArgs(_name, _thread.ManagedThreadId, _processor.GetType().FullName);

            ProcessorThreadStarting.Invoke(this, _eventArgs);

            while (!_thread.IsAlive && !CancellationToken.IsCancellationRequested)
            {
            }

            if (!CancellationToken.IsCancellationRequested)
            {
                ProcessorThreadActive.Invoke(this, _eventArgs);
            }

            _started = true;
        }

        public void Stop(TimeSpan timeout)
        {
            if (!_started)
            {
                throw new InvalidOperationException(Resources.ProcessorThreadNotStartedException);
            }

            ProcessorThreadStopping.Invoke(this, _eventArgs);

            _cancellationTokenSource.Cancel();

            _processor.AttemptDispose();

            if (_thread.IsAlive)
            {
                _thread.Join(timeout);
            }
        }

        private void Work()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                ProcessorExecuting.Invoke(this, _eventArgs);

                _processor.Execute(CancellationToken);
            }

            ProcessorThreadStopped.Invoke(this, _eventArgs);
        }

        internal void Deactivate()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}