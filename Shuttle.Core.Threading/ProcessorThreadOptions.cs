using System;
using System.Threading;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadOptions
    {
        public TimeSpan JoinTimeout { get; set; } = TimeSpan.FromSeconds(15);
        public bool IsBackground { get; set; } = true;
        public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;
    }
}