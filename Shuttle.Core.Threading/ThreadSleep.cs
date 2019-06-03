using System;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public static class ThreadSleep
    {
        public const int MaxStepSize = 1000;

        public static void While(int ms, CancellationToken cancellationToken)
        {
            // step size should be as large as possible,
            // max step size by default
            While(ms, cancellationToken, MaxStepSize);
        }

        public static void While(int ms, CancellationToken cancellationToken, int step)
        {
            Guard.AgainstNull(cancellationToken, nameof(cancellationToken));

            if (ms < 0)
            {
                return;
            }

            if (step < 0 || step > MaxStepSize)
            {
                step = MaxStepSize;
            }

            var end = DateTime.UtcNow.AddMilliseconds(ms);

            // sleep time should be:
            // - as large as possible to reduce burden on the os and improve accuracy
            // - less than the max step size to keep the thread responsive to thread state

            var remaining = (int) (end - DateTime.UtcNow).TotalMilliseconds;

            while (!cancellationToken.IsCancellationRequested && remaining > 0)
            {
                var sleep = remaining < step ? remaining : step;
                Thread.Sleep(sleep);
                remaining = (int) (end - DateTime.UtcNow).TotalMilliseconds;
            }
        }
    }
}