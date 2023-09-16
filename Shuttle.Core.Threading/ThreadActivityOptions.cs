using System;

namespace Shuttle.Core.Threading
{
    public class ThreadActivityOptions
    {
        public TimeSpan[] DurationToSleepWhenIdle { get; set; }
    }
}