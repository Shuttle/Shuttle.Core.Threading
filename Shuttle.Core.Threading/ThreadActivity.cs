using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ThreadActivity : IThreadActivity
    {
        private static readonly TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(250);
        private readonly TimeSpan[] _durations;

        private int _durationIndex;

        public ThreadActivity(IOptions<ThreadActivityOptions> threadActivityOptions)
        {
            Guard.AgainstNull(threadActivityOptions, nameof(threadActivityOptions));
            Guard.AgainstNull(threadActivityOptions.Value, nameof(threadActivityOptions.Value));

            _durations = threadActivityOptions.Value.DurationToSleepWhenIdle == null || threadActivityOptions.Value.DurationToSleepWhenIdle.Length == 0
                ? new[] { DefaultDuration }
                : threadActivityOptions.Value.DurationToSleepWhenIdle;
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