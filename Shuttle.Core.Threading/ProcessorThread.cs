using System;
using System.Threading;
using Shuttle.Core.Configuration;
using Shuttle.Core.Logging;

namespace Shuttle.Core.Threading
{
    public class ProcessorThread : IThreadState
    {
        private static readonly int ThreadJoinTimeoutInterval =
            ConfigurationItem<int>.ReadSetting("ThreadJoinTimeoutInterval", 1000).GetValue();

        private readonly ILog _log;
        private readonly string _name;
        private readonly IProcessor _processor;

        private volatile bool _active;

        private Thread _thread;

        public ProcessorThread(string name, IProcessor processor)
        {
            _name = name;
            _processor = processor;

            _log = Log.For(this);
        }

        public bool Active => _active;

        public void Start()
        {
            if (_active)
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
#if !NETCOREAPP2_0 && !NETCOREAPP2_1
                _log.Warning(ex.Message);
#else
                _log.Information(ex.Message);
#endif
            }

            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Normal;

            _active = true;

            _thread.Start();

            if (Log.IsTraceEnabled)
            {
                _log.Trace(string.Format(Resources.ProcessorThreadStarting, _thread.ManagedThreadId,
                    _processor.GetType().FullName));
            }

            while (!_thread.IsAlive && _active)
            {
            }

            if (_active && Log.IsTraceEnabled)
            {
                _log.Trace(string.Format(Resources.ProcessorThreadActive, _thread.ManagedThreadId,
                    _processor.GetType().FullName));
            }
        }

        public void Stop()
        {
            if (Log.IsTraceEnabled)
            {
                _log.Trace(string.Format(Resources.ProcessorThreadStopping, _thread.ManagedThreadId,
                    _processor.GetType().FullName));
            }

            _active = false;

            if (_thread.IsAlive)
            {
                _thread.Join(ThreadJoinTimeoutInterval);
            }
        }

        private void Work()
        {
            while (_active)
            {
                if (Log.IsVerboseEnabled)
                {
                    _log.Verbose(string.Format(Resources.ProcessorExecuting, _thread.ManagedThreadId,
                        _processor.GetType().FullName));
                }

                _processor.Execute(this);
            }

            if (Log.IsTraceEnabled)
            {
                _log.Trace(string.Format(Resources.ProcessorThreadStopped, _thread.ManagedThreadId,
                    _processor.GetType().FullName));
            }
        }

        internal void Deactivate()
        {
            _active = false;
        }
    }
}