using System;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ThreadState : IThreadState
    {
        private readonly Func<bool> _state;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ThreadState(CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNull(cancellationTokenSource, nameof(cancellationTokenSource));

            _cancellationTokenSource = cancellationTokenSource;
        }

        public ThreadState(Func<bool> state)
        {
            Guard.AgainstNull(state, nameof(state));

            _state = state;
        }

        public bool Active => _cancellationTokenSource?.IsCancellationRequested ?? _state.Invoke();
    }
}