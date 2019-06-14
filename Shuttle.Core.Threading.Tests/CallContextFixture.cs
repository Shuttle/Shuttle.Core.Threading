using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Threading.Tests
{
    [TestFixture]
    public class CallContextFixture
    {
        [Test]
        public void Should_be_able_to_flow_data()
        {
            var d1 = new object();
            var t1 = default(object);
            var t10 = default(object);
            var t11 = default(object);
            var t12 = default(object);
            var t13 = default(object);
            var d2 = new object();
            var t2 = default(object);
            var t20 = default(object);
            var t21 = default(object);
            var t22 = default(object);
            var t23 = default(object);

            Task.WaitAll(
                Task.Run(() =>
                {
                    CallContext.SetData("d1", d1);
                    new Thread(() => t10 = CallContext.GetData("d1")).Start();
                    Task.WaitAll(
                        Task.Run(() => t1 = CallContext.GetData("d1"))
                            .ContinueWith(t => Task.Run(() => t11 = CallContext.GetData("d1"))),
                        Task.Run(() => t12 = CallContext.GetData("d1")),
                        Task.Run(() => t13 = CallContext.GetData("d1"))
                    );
                }),
                Task.Run(() =>
                {
                    CallContext.SetData("d2", d2);
                    new Thread(() => t20 = CallContext.GetData("d2")).Start();
                    Task.WaitAll(
                        Task.Run(() => t2 = CallContext.GetData("d2"))
                            .ContinueWith(t => Task.Run(() => t21 = CallContext.GetData("d2"))),
                        Task.Run(() => t22 = CallContext.GetData("d2")),
                        Task.Run(() => t23 = CallContext.GetData("d2"))
                    );
                })
            );

            Assert.That(d1, Is.SameAs(t1));
            Assert.That(d1, Is.SameAs(t10));
            Assert.That(d1, Is.SameAs(t11));
            Assert.That(d1, Is.SameAs(t12));
            Assert.That(d1, Is.SameAs(t13));

            Assert.That(d2, Is.SameAs(t2));
            Assert.That(d2, Is.SameAs(t20));
            Assert.That(d2, Is.SameAs(t21));
            Assert.That(d2, Is.SameAs(t22));
            Assert.That(d2, Is.SameAs(t23));

            Assert.Null(CallContext.GetData("d1"));
            Assert.Null(CallContext.GetData("d2"));
        }
    }
}