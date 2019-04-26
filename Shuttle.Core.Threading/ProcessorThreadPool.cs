using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadPool : IProcessorThreadPool
    {
        private readonly ILog _log;
        private readonly string _name;
        private readonly IProcessorFactory _processorFactory;
        private readonly int _threadCount;
        private readonly List<ProcessorThread> _threads = new List<ProcessorThread>();
        private bool _disposed;
        private bool _started;

        public ProcessorThreadPool(string name, int threadCount, IProcessorFactory processorFactory)
        {
            Guard.AgainstNull(processorFactory, nameof(processorFactory));

            if (threadCount < 1)
            {
                throw new ThreadCountZeroException();
            }

            _name = name ?? Guid.NewGuid().ToString();
            _threadCount = threadCount;
            _processorFactory = processorFactory;

            _log = Log.For(this);
        }

        public void Pause()
        {
            foreach (var thread in _threads)
            {
                thread.Stop();
            }

            _log.Information(string.Format(Resources.ThreadPoolStatusChange, _name, "paused"));
        }

        public void Resume()
        {
            foreach (var thread in _threads)
            {
                thread.Start();
            }

            _log.Information(string.Format(Resources.ThreadPoolStatusChange, _name, "resumed"));
        }

        public IProcessorThreadPool Start()
        {
            if (_started)
            {
                return this;
            }

            StartThreads();

            _started = true;

            _log.Information(string.Format(Resources.ThreadPoolStatusChange, _name, "started"));

            return this;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void StartThreads()
        {
            var i = 0;

            while (i++ < _threadCount)
            {
                var thread = new ProcessorThread($"{_name} / {i}", _processorFactory.Create());

                _threads.Add(thread);

                thread.Start();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var thread in _threads)
                {
                    thread.Deactivate();
                }

                foreach (var thread in _threads)
                {
                    thread.Stop();
                }

                _processorFactory.AttemptDispose();
            }

            _disposed = true;
        }
    }
}