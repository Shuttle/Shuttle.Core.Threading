using System;

namespace Shuttle.Core.Threading
{
    public interface IThreadActivityConfiguration
    {
        TimeSpan[] DurationToSleepWhenIdle { get; }
    }
}