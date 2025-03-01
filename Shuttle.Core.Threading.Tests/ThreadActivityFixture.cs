using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Threading.Tests;

[TestFixture]
public class ThreadActivityFixture
{
    [Test]
    public async Task Should_be_able_to_have_the_thread_wait_async()
    {
        var activity = new ThreadActivity(
            new[]
            {
                TimeSpan.FromMilliseconds(250),
                TimeSpan.FromMilliseconds(500)
            });

        var start = DateTimeOffset.UtcNow;
        var token = new CancellationToken(false);

        await activity.WaitingAsync(token);

        Assert.That((DateTimeOffset.UtcNow - start).TotalMilliseconds >= 250, Is.True);

        await activity.WaitingAsync(token);

        Assert.That((DateTimeOffset.UtcNow - start).TotalMilliseconds >= 750, Is.True);
    }
}