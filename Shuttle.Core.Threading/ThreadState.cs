using System;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ThreadState : IThreadState
    {
        private readonly Func<bool> _state;
        private readonly CancellationToken _cancellationToken;

        public ThreadState(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        public ThreadState(Func<bool> state)
        {
            Guard.AgainstNull(state, nameof(state));

            _state = state;
        }

        public bool Active => _state?.Invoke() ?? _cancellationToken.IsCancellationRequested;
    }
}