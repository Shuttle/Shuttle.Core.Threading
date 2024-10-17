﻿using System.Threading;

namespace Shuttle.Core.Threading;

public interface ICancellationTokenSource
{
    CancellationTokenSource Get();
    void Renew();
}