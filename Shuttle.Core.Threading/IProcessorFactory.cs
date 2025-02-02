namespace Shuttle.Core.Threading;

public interface IProcessorFactory
{
    IProcessor Create();
}