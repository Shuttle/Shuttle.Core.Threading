using System;

namespace Shuttle.Core.Threading
{
    public interface IProcessorThreadPool : IDisposable
    {
        void Pause();
        void Resume();
        IProcessorThreadPool Start();
    }
}