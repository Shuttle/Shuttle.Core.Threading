namespace Shuttle.Core.Threading;

public interface IProcessorThreadContext
{
    object? GetState(string key);
}