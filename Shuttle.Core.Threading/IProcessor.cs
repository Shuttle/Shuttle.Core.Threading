namespace Shuttle.Core.Threading
{
    public interface IProcessor
    {
        void Execute(IThreadState state);
    }
}