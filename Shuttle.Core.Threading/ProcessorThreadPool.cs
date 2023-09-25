using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public void Stop()
        {
            foreach (var thread in _threads)
            {
                thread.Stop(_joinTimeout);
            }
        }

        public IProcessorThreadPool Start()
        {
            Start(true).GetAwaiter().GetResult();

            return this;
        }

        public async Task<IProcessorThreadPool> StartAsync()
        {
            await Start(false);

            return this;
        }

        public IEnumerable<ProcessorThread> ProcessorThreads => _threads.AsReadOnly();

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private async Task Start(bool sync)
        {
            if (_started)
            {
                return;
            }

            var i = 0;

            while (i++ < _threadCount)
            {
                var thread = new ProcessorThread($"{_name} / {i}", _processorFactory.Create(), _processorThreadOptions);

                _threads.Add(thread);

                if (sync)
                {
                    thread.Start();
                }
                else
                {
                    await thread.StartAsync();
                }
            }

            _started = true;
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

                _processorFactory.TryDispose();
            }

            _disposed = true;
        }
    }
}