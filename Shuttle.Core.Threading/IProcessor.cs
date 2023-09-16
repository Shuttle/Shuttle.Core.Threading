using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Threading
{
    public interface IProcessor
    {
        void Execute(CancellationToken cancellationToken);
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}