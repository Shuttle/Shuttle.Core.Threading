using System;
using System.Threading;
using Shuttle.Core.Configuration;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Threading
{
    public class ProcessorThread
    {
        private static readonly int ThreadJoinTimeoutInterval =
            ConfigurationItem<int>.ReadSetting("ThreadJoinTimeoutInterval", 1000).GetValue();

        private readonly ILog _log;
        private readonly string _name;
        private readonly IProcessor _processor;
        private readonly System.Threading.CancellationTokenSource _cancellationTokenSource = new System.Threading.CancellationTokenSource();

        private bool _started;

        private Thread _thread;

        public ProcessorThread(string name, IProcessor processor)
        {
            Guard.AgainstNull(processor, nameof(processor));

            _name = name;
            _processor = processor;

            CancellationToken = _cancellationTokenSource.Token;

            _log = Log.For(this);
        }

        public CancellationToken CancellationToken {get; }

        public void Start()
        {
            if (_started)
            {
                return;
            }

            _thread = new Thread(Work) {Name = _name};

            try
            {
                _thread.SetApartmentState(ApartmentState.MTA);
            }
            catch (Exception ex)
            {
#if !NETCOREAPP2_1
                _log.Warning(ex.Message);
#else
                _log.Information(ex.Message);
#endif
            }

            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Normal;

            _thread.Start();

            if (Log.IsTraceEnabled)
            {
                _log.Trace(string.Format(Resources.ProcessorThreadStarting, _thread.ManagedThreadId,
                    _processor.GetType().FullName));
            }

            while (!_thread.IsAlive && !CancellationToken.IsCancellationRequested)
            {
            }

            if (!CancellationToken.IsCancellationRequested && Log.IsTraceEnabled)
            {
                _log.Trace(string.Format(Resources.ProcessorThreadActive, _thread.ManagedThreadId,
                    _processor.GetType().FullName));
            }

            _started = true;
        }

        public void Stop()
        {
            if (Log.IsTraceEnabled)
            {
                _log.Trace(string.Format(Resources.ProcessorThreadStopping, _thread.ManagedThreadId,
                    _processor.GetType().FullName));
            }

            _cancellationTokenSource.Cancel();

            _processor.AttemptDispose();

            if (_thread.IsAlive)
            {
                _thread.Join(ThreadJoinTimeoutInterval);
            }
        }

        private void Work()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                if (Log.IsVerboseEnabled)
                {
                    _log.Verbose(string.Format(Resources.ProcessorExecuting, _thread.ManagedThreadId,
                        _processor.GetType().FullName));
                }

                _processor.Execute(CancellationToken);
            }

            if (Log.IsTraceEnabled)
            {
                _log.Trace(string.Format(Resources.ProcessorThreadStopped, _thread.ManagedThreadId,
                    _processor.GetType().FullName));
            }
        }

        internal void Deactivate()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}