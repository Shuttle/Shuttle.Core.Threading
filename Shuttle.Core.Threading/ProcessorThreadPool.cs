using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadPool : IProcessorThreadPool
    {
        public string Name { get; }
        public IProcessorFactory ProcessorFactory { get; }
        public ProcessorThreadOptions ThreadOptions { get; }
        public int ThreadCount { get; }

        private readonly List<ProcessorThread> _processorThreads = new List<ProcessorThread>();
        private bool _disposed;
        private bool _started;

        public ProcessorThreadPool(string name, int threadCount, IProcessorFactory processorFactory, ProcessorThreadOptions processorThreadOptions)
        {
            Guard.AgainstNull(processorFactory, nameof(processorFactory));
            Guard.AgainstNull(processorThreadOptions, nameof(processorThreadOptions));

            if (threadCount < 1)
            {
                throw new ThreadCountZeroException();
            }

            Name = name ?? Guid.NewGuid().ToString();
            ProcessorFactory = processorFactory;
            ThreadOptions = processorThreadOptions;
            ThreadCount = threadCount;
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

            while (i++ < ThreadCount)
            {
                var processorThread = new ProcessorThread($"{Name} / {i}", ProcessorFactory.Create(), ThreadOptions);

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

                ProcessorFactory.TryDispose();
            }

            _disposed = true;
        }
    }
}