using NUnit.Framework;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Moq;

namespace Shuttle.Core.Threading.Tests;

public class ProcessorThreadPoolFixture
{
    [Test]
    public void Should_be_able_to_execute_processor_thread_pool()
    {
        Should_be_able_to_execute_processor_thread_pool_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_execute_processor_thread_pool_async()
    {
        await Should_be_able_to_execute_processor_thread_pool_async(false);
    }

    private async Task Should_be_able_to_execute_processor_thread_pool_async(bool sync)
    {
        const int minimumExecutionCount = 5;

        var executionDuration = TimeSpan.FromMilliseconds(500);
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var processorFactory = new Mock<IProcessorFactory>();

        processorFactory.Setup(m => m.Create()).Returns(() => new MockProcessor(executionDuration));

        var processorThreadPool = new ProcessorThreadPool("thread-pool", 5, processorFactory.Object, new ProcessorThreadOptions());

        processorThreadPool.ProcessorThreadCreated += (sender, args) =>
        {
            args.ProcessorThread.ProcessorException += (sender, args) =>
            {
                Console.WriteLine($@"{DateTime.Now:O} - [ProcessorException] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId} / exception = '{args.Exception}'");
            };

            args.ProcessorThread.ProcessorExecuting += (sender, args) =>
            {
                Console.WriteLine($@"{DateTime.Now:O} - [ProcessorExecuting] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };

            args.ProcessorThread.ProcessorThreadActive += (sender, args) =>
            {
                Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadActive] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };

            args.ProcessorThread.ProcessorThreadStarting += (sender, args) =>
            {
                Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadStarting] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };

            args.ProcessorThread.ProcessorThreadStopped += (sender, args) =>
            {
                Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadStopped] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId} / aborted = '{args.Aborted}'");
            };

            args.ProcessorThread.ProcessorThreadStopping += (sender, args) =>
            {
                Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadStopping] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };

            args.ProcessorThread.ProcessorThreadOperationCanceled += (sender, args) =>
            {
                Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadOperationCanceled] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
            };
        };

        if (sync)
        {
            processorThreadPool.Start();
        }
        else
        {
            await processorThreadPool.StartAsync();
        }

        var timeout = DateTime.Now.AddSeconds(5);
        var timedOut = false;

        while (processorThreadPool.ProcessorThreads.Any(item => ((MockProcessor)item.Processor).ExecutionCount <= minimumExecutionCount && !timedOut))
        {
            await Task.Delay(25, cancellationToken).ConfigureAwait(false);

            timedOut = DateTime.Now >= timeout;
        }

        cancellationTokenSource.Cancel();

        processorThreadPool.Stop();

        Assert.That(timedOut, Is.False, $"[TIMEOUT] : Did not complete {minimumExecutionCount} executions before {timeout:O}");
    }
}