using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly Dictionary<string, SemaphoreSlim> _semaphores = new Dictionary<string, SemaphoreSlim>();

        public async Task Wait(string name, CancellationToken cancellationToken = default)
        {
            if (!_semaphores.ContainsKey(Guard.AgainstNullOrEmptyString(name, nameof(name))))
            {
                _semaphores.Add(name, new SemaphoreSlim(1, 1));
            }

            await _semaphores[name].WaitAsync(cancellationToken);
        }

        public void Release(string name)
        {
            if (!_semaphores.ContainsKey(Guard.AgainstNullOrEmptyString(name, nameof(name))))
            {
                throw new ApplicationException(string.Format(Resources.SynchronizationNameException, name));
            }

            _semaphores[name].Release();
        }
    }
}