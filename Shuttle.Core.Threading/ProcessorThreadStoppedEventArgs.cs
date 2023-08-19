namespace Shuttle.Core.Threading
{
    public class ProcessorThreadStoppedEventArgs : ProcessorThreadEventArgs
    {
        public bool Aborted { get; }

        public ProcessorThreadStoppedEventArgs(string name, int managedThreadId, string processorTypeFullName, bool aborted) 
            : base(name, managedThreadId, processorTypeFullName)
        {
            Aborted = aborted;
        }
    }
}