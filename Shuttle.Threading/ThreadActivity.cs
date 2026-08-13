using Shuttle.Contract;

namespace Shuttle.Threading;

public class ThreadActivity(IEnumerable<TimeSpan> durations) : IThreadActivity
{
    private readonly TimeSpan[] _durations = Guard.AgainstEmpty(durations).ToArray();

    // A single `ThreadActivity` instance is shared by every processor thread that uses the same service key, so the
    // duration index has to be guarded; without it two threads may both pass the bounds check and the second would
    // then index beyond the end of the array.
    private readonly Lock _lock = new();

    private int _durationIndex;

    public async Task SignalAsync(bool workPerformed, CancellationToken cancellationToken)
    {
        TimeSpan sleepTimeSpan;

        lock (_lock)
        {
            if (workPerformed)
            {
                _durationIndex = 0;

                return;
            }

            sleepTimeSpan = GetSleepTimeSpan();
        }

        await Task.Delay(sleepTimeSpan, cancellationToken).ConfigureAwait(false);
    }

    private TimeSpan GetSleepTimeSpan()
    {
        if (_durationIndex >= _durations.Length)
        {
            _durationIndex = _durations.Length - 1;
        }

        return _durations[_durationIndex++];
    }
}