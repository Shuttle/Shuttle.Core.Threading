using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading
{
    public interface IThreadActivity
    {
        Task Waiting(CancellationToken cancellationToken);
        void Working();
    }
}