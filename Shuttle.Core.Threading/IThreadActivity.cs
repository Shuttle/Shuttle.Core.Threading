using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading
{
    public interface IThreadActivity
    {
        void Waiting(CancellationToken cancellationToken);
        Task WaitingAsync(CancellationToken cancellationToken);
        void Working();
    }
}