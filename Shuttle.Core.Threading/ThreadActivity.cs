using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ThreadActivity : IThreadActivity
    {
        private static readonly TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(250);
        private readonly TimeSpan[] _durations;

        private int _durationIndex;

        public ThreadActivity(IEnumerable<TimeSpan> durationToSleepWhenIdle)
        {
            Guard.AgainstNull(durationToSleepWhenIdle, nameof(durationToSleepWhenIdle));

            _durations = durationToSleepWhenIdle.ToArray();
            _durationIndex = 0;
        }

        public ThreadActivity(IThreadActivityConfiguration threadActivityConfiguration)
        {
            Guard.AgainstNull(threadActivityConfiguration, nameof(threadActivityConfiguration));

            _durations = threadActivityConfiguration.DurationToSleepWhenIdle;
            _durationIndex = 0;
        }

        public async Task Waiting(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(GetSleepTimeSpan(), cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void Working()
        {
            _durationIndex = 0;
        }

        private TimeSpan GetSleepTimeSpan()
        {
            if (_durations == null || _durations.Length == 0)
            {
                return DefaultDuration;
            }

            if (_durationIndex >= _durations.Length)
            {
                _durationIndex = _durations.Length - 1;
            }

            return _durations[_durationIndex++];
        }
    }
}