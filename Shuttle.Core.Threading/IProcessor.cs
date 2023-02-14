using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading
{
    public interface IProcessor
    {
        Task Execute(CancellationToken cancellationToken);
    }
}