using System;

namespace Shuttle.Core.Threading;

public class ThreadCountZeroException : Exception
{
    public ThreadCountZeroException()
        : base(Resources.ThreadCountZeroException)
    {
    }
}