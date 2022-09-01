using System;
using System.Collections.Generic;

namespace Shuttle.Core.Threading
{
    public interface IProcessorThreadPool : IDisposable
    {
        void Pause();
        void Resume();
        IProcessorThreadPool Start();
        IEnumerable<ProcessorThread> ProcessorThreads { get; }
    }
}