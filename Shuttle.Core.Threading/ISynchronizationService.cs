using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading
{
    public interface ISynchronizationService
    {
        Task WaitAsync(string name, CancellationToken cancellationToken = default);
        void Release(string name);
    }
}