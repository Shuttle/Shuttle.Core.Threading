using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading;

public interface IThreadActivity
{
    Task WaitingAsync(CancellationToken cancellationToken = default);
    void Working();
}