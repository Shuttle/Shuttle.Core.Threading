# Shuttle.Core.Threading

```
PM> Install-Package Shuttle.Core.Threading
```

Provides various classes and interfaces to facilitate thread-based processing.

## ProcessorThreadPool

``` c#
public ProcessorThreadPool(
    string name, 
    int threadCount, 
    IServiceScopeFactory serviceScopeFactory,
    IProcessorFactory processorFactory, 
    ProcessorThreadOptions processorThreadOptions
);
```

Each thread pool has a `name` used only for identyfing the pool.  The `threadCount` determines the number of `ProcessorThread` instances in the pool.  Each `ProcessorThread` calls the `IProcessor.ExecuteAsync(CancellationToken)` method on the instance provided by the `IProcessorFactory.Create()` method in a loop while the `CancellationToken.IsCancellationRequested` returns `false`.

Every call to `IProcessor.ExecuteAsync(ProcessorThreadContext, CancellationToken)` is wrapped in a `ProcessorThreadContext` instance that provides the `State` along with the `IServiceScope` instance created by the `IServiceScopeFactory.CreateScope()` method.

## ProcessorThreadOptions

| Option | Default | Description |
| --- | --- | --- |
| `JoinTimeout` | `00:00:15` | The duration to allow the processor thread to join the main thread. |
| `IsBackground` | `true` | Indicates whether the thread will be started as a background thread.  Background threads are instantly killed when the host process stops. |
| `Priority` | `ThreadPriority.Normal` | Indicates the [thread priority](https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread.priority?view=net-6.0). |