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
        private readonly TimeSpan[] _durations;

        private int _durationIndex;

        public ThreadActivity(IEnumerable<TimeSpan> waitDurations)
        {
            Guard.AgainstEmptyEnumerable(waitDurations, nameof(waitDurations));

            _durations = waitDurations.ToArray();
            _durationIndex = 0;
        }

        public void Waiting(CancellationToken cancellationToken)
        {
            try
            {
                Task.Delay(GetSleepTimeSpan(), cancellationToken).Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public async Task WaitingAsync(CancellationToken cancellationToken)
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
            if (_durationIndex >= _durations.Length)
            {
                _durationIndex = _durations.Length - 1;
            }

            return _durations[_durationIndex++];
        }
    }
}