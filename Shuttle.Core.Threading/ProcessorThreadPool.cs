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
        private readonly ProcessorThreadOptions _processorThreadOptions;
        private readonly List<ProcessorThread> _threads = new List<ProcessorThread>();
        private bool _disposed;
        private bool _started;
        private readonly TimeSpan _joinTimeout;
        private readonly int _threadCount;

        public ProcessorThreadPool(string name, int threadCount, IProcessorFactory processorFactory, ProcessorThreadOptions processorThreadOptions)
        {
            Guard.AgainstNull(processorFactory, nameof(processorFactory));
            Guard.AgainstNull(processorThreadOptions, nameof(processorThreadOptions));

            if (threadCount < 1)
            {
                throw new ThreadCountZeroException();
            }

            _name = name ?? Guid.NewGuid().ToString();
            _processorFactory = processorFactory;
            _processorThreadOptions = processorThreadOptions;

            _joinTimeout = _processorThreadOptions.JoinTimeout;
            _threadCount = threadCount;

            if (_joinTimeout.TotalSeconds < 1)
            {
                _joinTimeout = TimeSpan.FromSeconds(1);
            }
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

        public IEnumerable<ProcessorThread> ProcessorThreads => _threads.AsReadOnly();

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
                var thread = new ProcessorThread($"{_name} / {i}", _processorFactory.Create(), _processorThreadOptions);

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