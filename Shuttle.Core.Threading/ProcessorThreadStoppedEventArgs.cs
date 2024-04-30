namespace Shuttle.Core.Threading
{
    public class ProcessorThreadStoppedEventArgs : ProcessorThreadEventArgs
    {
        public bool Aborted { get; }

        public ProcessorThreadStoppedEventArgs(string name, int managedThreadId, bool aborted) 
            : base(name, managedThreadId)
        {
            Aborted = aborted;
        }
    }
}