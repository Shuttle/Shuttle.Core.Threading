using System;
using System.Threading;

namespace Shuttle.Core.Threading
{
    public class DefaultCancellationTokenSource : ICancellationTokenSource, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public CancellationTokenSource Get()
        {
            return _cancellationTokenSource;
        }

        public void Renew()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}