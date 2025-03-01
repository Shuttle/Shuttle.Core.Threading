using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Threading.Tests;

public class ProcessorThreadPoolFixture
{
    [Test]
    public async Task Should_be_able_to_execute_processor_thread_pool_async()
    {
        const int minimumExecutionCount = 5;

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();

        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(new Mock<IServiceScope>().Object);

        var executionDuration = TimeSpan.FromMilliseconds(500);
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var processorFactory = new Mock<IProcessorFactory>();

        processorFactory.Setup(m => m.Create()).Returns(() => new MockProcessor(executionDuration));

        var processorThreadPool = new ProcessorThreadPool("thread-pool", 5, serviceScopeFactory.Object, processorFactory.Object, new());

        processorThreadPool.ProcessorThreadCreated += (_, args) =>
        {
            args.ProcessorThread.ProcessorException += (sender, args) =>
            {
                Console.WriteLine($@"{DateTimeOffset.UtcNow:O} - [ProcessorException] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId} / exception = '{args.Exception}'");
            };

            args.ProcessorThread.ProcessorExecuting += (sender, args) =>
            {
                Console.WriteLine($@"{DateTimeOffset.UtcNow:O} - [ProcessorExecuting] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };

            args.ProcessorThread.ProcessorThreadActive += (sender, args) =>
            {
                Console.WriteLine($@"{DateTimeOffset.UtcNow:O} - [ProcessorThreadActive] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };

            args.ProcessorThread.ProcessorThreadStarting += (sender, args) =>
            {
                Console.WriteLine($@"{DateTimeOffset.UtcNow:O} - [ProcessorThreadStarting] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };

            args.ProcessorThread.ProcessorThreadStopped += (sender, args) =>
            {
                Console.WriteLine($@"{DateTimeOffset.UtcNow:O} - [ProcessorThreadStopped] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };

            args.ProcessorThread.ProcessorThreadStopping += (sender, args) =>
            {
                Console.WriteLine($@"{DateTimeOffset.UtcNow:O} - [ProcessorThreadStopping] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };

            args.ProcessorThread.ProcessorThreadOperationCanceled += (sender, args) =>
            {
                Console.WriteLine($@"{DateTimeOffset.UtcNow:O} - [ProcessorThreadOperationCanceled] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };
        };

        await processorThreadPool.StartAsync();

        var timeout = DateTimeOffset.UtcNow.AddSeconds(5);
        var timedOut = false;

        while (processorThreadPool.ProcessorThreads.Any(item => ((MockProcessor)item.Processor).ExecutionCount <= minimumExecutionCount && !timedOut))
        {
            await Task.Delay(25, cancellationToken).ConfigureAwait(false);

            timedOut = DateTimeOffset.UtcNow >= timeout;
        }

        cancellationTokenSource.Cancel();

        await processorThreadPool.StopAsync();

        Assert.That(timedOut, Is.False, $"[TIMEOUT] : Did not complete {minimumExecutionCount} executions before {timeout:O}");
    }
}