using Shuttle.Contract;

namespace Shuttle.Threading;

public class ThreadActivity(IEnumerable<TimeSpan> durations) : IThreadActivity
{
    private readonly TimeSpan[] _durations = Guard.AgainstEmpty(durations).ToArray();

    // A single `ThreadActivity` instance is shared by every processor thread that uses the same service key, so
    // the idle state has to be guarded. It is also why the ladder position is derived from wall-clock time rather
    // than a per-call counter: with N threads all signalling idleness concurrently, a counter advances N times
    // per "round", so the ladder would reach its final (longest) duration almost immediately regardless of the
    // durations configured — instead of only after that much real idle time has actually elapsed. Time-based
    // escalation still means the *aggregate* poll rate across N idle threads bottoms out at roughly
    // (shortest-ever-reached duration / N), since each thread sleeps and wakes independently — that is inherent
    // to running N concurrent pollers and is controlled via thread count, not this class.
    private readonly Lock _lock = new();

    private DateTimeOffset? _idleSince;

    public async Task SignalAsync(bool workPerformed, CancellationToken cancellationToken)
    {
        TimeSpan sleepTimeSpan;

        lock (_lock)
        {
            if (workPerformed)
            {
                _idleSince = null;

                return;
            }

            _idleSince ??= DateTimeOffset.UtcNow;

            sleepTimeSpan = GetSleepTimeSpan(DateTimeOffset.UtcNow - _idleSince.Value);
        }

        await Task.Delay(sleepTimeSpan, cancellationToken).ConfigureAwait(false);
    }

    private TimeSpan GetSleepTimeSpan(TimeSpan elapsedSinceIdle)
    {
        var cumulative = TimeSpan.Zero;

        foreach (var duration in _durations)
        {
            cumulative += duration;

            if (elapsedSinceIdle < cumulative)
            {
                return duration;
            }
        }

        return _durations[^1];
    }
}