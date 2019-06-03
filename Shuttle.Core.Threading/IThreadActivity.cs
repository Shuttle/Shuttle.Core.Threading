using System.Threading;

namespace Shuttle.Core.Threading
{
    public interface IThreadActivity
    {
        void Waiting(CancellationToken cancellationToken);
        void Working();
    }
}