using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    public class ProcessorThreadCreatedEventArgs : System.EventArgs
    {
        public ProcessorThread ProcessorThread { get; }

        public ProcessorThreadCreatedEventArgs(ProcessorThread processorThread)
        {
            ProcessorThread = Guard.AgainstNull(processorThread, nameof(processorThread));
        }
    }
}