namespace Shuttle.Core.Threading
{
    public interface IThreadActivity
    {
        void Waiting(IThreadState state);
        void Working();
    }
}