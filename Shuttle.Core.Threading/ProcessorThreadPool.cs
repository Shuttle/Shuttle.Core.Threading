using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadPool : IProcessorThreadPool
    {
        private readonly string _name;
        private readonly IProcessorFactory _processorFactory;
        private readonly int _threadCount;
        private readonly TimeSpan _joinTimeout;
        private readonly List<ProcessorThread> _threads = new List<ProcessorThread>();
        private bool _disposed;
        private bool _started;

        public ProcessorThreadPool(string name, int threadCount, TimeSpan joinTimeout, IProcessorFactory processorFactory)
        {
            Guard.AgainstNull(processorFactory, nameof(processorFactory));

            if (threadCount < 1)
            {
                throw new ThreadCountZeroException();
            }

            _name = name ?? Guid.NewGuid().ToString();
            _threadCount = threadCount;
            _joinTimeout = joinTimeout;
            _processorFactory = processorFactory;
        }

        public void Pause()
        {
            foreach (var thread in _threads)
            {
                thread.Stop(_joinTimeout);
            }
        }

        public void Resume()
        {
            foreach (var thread in _threads)
            {
                thread.Start();
            }
        }

        public IProcessorThreadPool Start()
        {
            if (_started)
            {
                return this;
            }

            StartThreads();

            _started = true;

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
                    thread.Stop(_joinTimeout);
                }

                _processorFactory.AttemptDispose();
            }

            _disposed = true;
        }
    }
}