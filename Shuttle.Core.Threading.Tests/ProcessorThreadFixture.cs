using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Threading.Tests;

public class ProcessorThreadFixture
{
    [Test]
    public void Should_be_able_to_execute_processor_thread()
    {
        Should_be_able_to_execute_processor_thread_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_execute_processor_thread_async()
    {
        await Should_be_able_to_execute_processor_thread_async(false);
    }

    private async Task Should_be_able_to_execute_processor_thread_async(bool sync)
    {
        const int minimumExecutionCount = 5;

        var executionDuration = TimeSpan.FromMilliseconds(200);
        var mockProcessor = new MockProcessor(executionDuration);
        var processorThread = new ProcessorThread("thread", mockProcessor, new ProcessorThreadOptions());
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        processorThread.ProcessorException += (sender, args) =>
        {
            Console.WriteLine($@"{DateTime.Now:O} - [ProcessorException] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId} / exception = '{args.Exception}'");
        };

        processorThread.ProcessorExecuting += (sender, args) =>
        {
            Console.WriteLine($@"{DateTime.Now:O} - [ProcessorExecuting] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
        };

        processorThread.ProcessorThreadActive += (sender, args) =>
        {
            Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadActive] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
        };

        processorThread.ProcessorThreadStarting += (sender, args) =>
        {
            Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadStarting] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
        };

        processorThread.ProcessorThreadStopped += (sender, args) =>
        {
            Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadStopped] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId} / aborted = '{args.Aborted}'");
        };

        processorThread.ProcessorThreadStopping += (sender, args) =>
        {
            Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadStopping] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
        };

        processorThread.ProcessorThreadOperationCanceled += (sender, args) =>
        {
            Console.WriteLine($@"{DateTime.Now:O} - [ProcessorThreadOperationCanceled] : name = '{args.Name}' / execution count = {((MockProcessor)((ProcessorThread)sender).Processor).ExecutionCount} / managed thread id = {args.ManagedThreadId}");
        };


        if (sync)
        {
            processorThread.Start();
        }
        else
        {
            await processorThread.StartAsync();
        }

        var timeout = DateTime.Now.AddSeconds(500);
        var timedOut = false;

        while (mockProcessor.ExecutionCount <= minimumExecutionCount && !timedOut)
        {
            await Task.Delay(25, cancellationToken).ConfigureAwait(false);

            timedOut = DateTime.Now >= timeout;
        }

        cancellationTokenSource.Cancel();

        processorThread.Stop();

        Assert.That(timedOut, Is.False, $"[TIMEOUT] : Did not complete {minimumExecutionCount} executions before {timeout:O}");
    }
}