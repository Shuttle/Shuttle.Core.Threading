using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading
{
    public interface IProcessorThreadPool : IDisposable
    {
        void Pause();
        void Resume();
        IProcessorThreadPool Start();
        IProcessorThreadPool StartAsync();
        IEnumerable<ProcessorThread> ProcessorThreads { get; }
    }
}