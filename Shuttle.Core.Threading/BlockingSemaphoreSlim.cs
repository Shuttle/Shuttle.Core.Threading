using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class BlockingSemaphoreSlim : IBlockingSemaphore
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public BlockingSemaphoreSlim(SemaphoreSlim semaphoreSlim)
        {
            _semaphoreSlim = Guard.AgainstNull(semaphoreSlim, nameof(semaphoreSlim));
        }

        public void Release()
        {
            _semaphoreSlim.Release();
        }
    }
}