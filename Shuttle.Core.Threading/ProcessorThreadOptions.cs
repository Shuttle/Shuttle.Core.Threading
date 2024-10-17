using System;
using System.Threading;

namespace Shuttle.Core.Threading;

public class ProcessorThreadOptions
{
    public bool IsBackground { get; set; } = true;
    public TimeSpan JoinTimeout { get; set; } = TimeSpan.FromSeconds(15);
    public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;
}