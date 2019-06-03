using System.Threading;

namespace Shuttle.Core.Threading
{
    public interface IProcessor
    {
        void Execute(CancellationToken cancellationToken);
    }
}