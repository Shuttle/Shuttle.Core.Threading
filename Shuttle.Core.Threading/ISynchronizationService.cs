using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading
{
    public interface ISynchronizationService
    {
        Task Wait(string name, CancellationToken cancellationToken = default);
        void Release(string name);
    }
}