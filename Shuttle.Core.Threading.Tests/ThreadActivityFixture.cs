using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Threading.Tests
{
    [TestFixture]
    public class ThreadActivityFixture
    {
        [Test]
        public async Task Should_be_able_to_have_the_thread_wait()
        {
            var activity = new ThreadActivity(new[]
            {
                TimeSpan.FromMilliseconds(250),
                TimeSpan.FromMilliseconds(500)
            });

            var start = DateTime.Now;
            var token = new CancellationToken(false);

            await activity.WaitingAsync(token);

            Assert.IsTrue((DateTime.Now - start).TotalMilliseconds >= 250);

            await activity.WaitingAsync(token);

            Assert.IsTrue((DateTime.Now - start).TotalMilliseconds >= 750);
        }
    }
}