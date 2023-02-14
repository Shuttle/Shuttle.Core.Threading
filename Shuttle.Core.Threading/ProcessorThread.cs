using System;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Threading
{
    public class ProcessorThread
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly string _name;
        private readonly IProcessor _processor;
        private readonly ProcessorThreadOptions _processorThreadOptions;
        private ProcessorThreadEventArgs _eventArgs;

        private bool _started;
        private Thread _thread;

        public ProcessorThread(string name, IProcessor processor, ProcessorThreadOptions processorThreadOptions)
        {
            Guard.AgainstNull(processor, nameof(processor));
            Guard.AgainstNull(processorThreadOptions, nameof(processorThreadOptions));

            _name = name;
            _processor = processor;
            _processorThreadOptions = processorThreadOptions;

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

            _thread.IsBackground = _processorThreadOptions.IsBackground;
            _thread.Priority = _processorThreadOptions.Priority;

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

            _processor.TryDispose();

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

                _processor.Execute(CancellationToken).GetAwaiter().GetResult();
            }

            ProcessorThreadStopped.Invoke(this, _eventArgs);
        }

        internal void Deactivate()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}