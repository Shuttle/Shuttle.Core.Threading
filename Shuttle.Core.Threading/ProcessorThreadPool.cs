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
        private readonly List<ProcessorThread> _processorThreads = new List<ProcessorThread>();
        private bool _disposed;
        private bool _started;
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
            _threadCount = threadCount;
        }

        public event EventHandler<ProcessorThreadCreatedEventArgs> ProcessorThreadCreated;

        public void Stop()
        {
            foreach (var thread in _processorThreads)
            {
                thread.Stop();
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

        public IEnumerable<ProcessorThread> ProcessorThreads => _processorThreads.AsReadOnly();

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
                var processorThread = new ProcessorThread($"{_name} / {i}", _processorFactory.Create(), _processorThreadOptions);

                ProcessorThreadCreated?.Invoke(this, new ProcessorThreadCreatedEventArgs(processorThread));

                _processorThreads.Add(processorThread);

                if (sync)
                {
                    processorThread.Start();
                }
                else
                {
                    await processorThread.StartAsync();
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
                foreach (var thread in _processorThreads)
                {
                    thread.Deactivate();
                }

                foreach (var thread in _processorThreads)
                {
                    thread.Stop();
                }

                _processorFactory.TryDispose();
            }

            _disposed = true;
        }
    }
}