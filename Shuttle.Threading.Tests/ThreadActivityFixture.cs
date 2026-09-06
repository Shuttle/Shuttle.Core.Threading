using System.Diagnostics;
using NUnit.Framework;

namespace Shuttle.Threading.Tests;

[TestFixture]
public class ThreadActivityFixture
{
    [Test]
    public async Task Should_be_able_to_have_the_thread_wait_async()
    {
        var activity = new ThreadActivity(
        [
            TimeSpan.FromMilliseconds(250),
            TimeSpan.FromMilliseconds(500)
        ]);

        var start = DateTime.Now;
        var token = new CancellationToken(false);

        await activity.SignalAsync(false, token);

        Assert.That((DateTime.Now - start).TotalMilliseconds >= 250, Is.True);

        await activity.SignalAsync(false, token);

        Assert.That((DateTime.Now - start).TotalMilliseconds >= 750, Is.True);
    }

    [Test]
    public async Task Should_give_every_concurrent_caller_the_same_early_duration_when_idle_starts()
    {
        // A single `ThreadActivity` is shared by every processor thread using the same service key. With a
        // call-count-based ladder, N threads all signalling idleness for the first time at roughly the same
        // moment would race each other through the ladder: only the first caller gets the short entry, and
        // every other concurrent caller is immediately pushed to the last (longest) entry - even though, from
        // each thread's own point of view, it has only just gone idle. Escalating by elapsed wall-clock time
        // instead means every caller within the same idle window gets the same (short) duration.
        var activity = new ThreadActivity(
        [
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromSeconds(5)
        ]);

        var token = new CancellationToken(false);
        var elapsed = Stopwatch.StartNew();

        await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => activity.SignalAsync(false, token)));

        elapsed.Stop();

        Assert.That(elapsed.Elapsed.TotalSeconds, Is.LessThan(1));
    }
}