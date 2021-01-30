﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Threading.Tests
{
    [TestFixture]
    public class SharedCancellationTokenSourceFixture
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        [Test]
        public void Should_be_able_to_shared_cancellation_token_source_entries()
        {
            using (var cancellationTokenSource = new DefaultCancellationTokenSource())
            {
                Assert.That(cancellationTokenSource.Get(), Is.SameAs(cancellationTokenSource.Get()));

                var tasks = new List<Task>
                {
                    Task.Run(() => Spin("A", cancellationTokenSource.Get().Token)),
                    Task.Run(() => Spin("B", cancellationTokenSource.Get().Token)),
                    Task.Run(() => Spin("C", cancellationTokenSource.Get().Token)),
                    Task.Run(() => Spin("D", cancellationTokenSource.Get().Token))
                };

                // wait for all the tasks to start
                while (tasks.Any(task => task.Status != TaskStatus.Running))
                {
                    Thread.Sleep(100);
                }

                cancellationTokenSource.Get().Cancel();

                Task.WaitAll(tasks.ToArray());
            }
        }

        private void Spin(string name, CancellationToken cancellationToken)
        {
            Log($@"[starting] : {name}");

            while (!cancellationToken.IsCancellationRequested)
            {
                var timeout = Random.Next(5, 20) * 100;

                Log($@"[sleeping] : {name} / timeout = {timeout}");

                Thread.Sleep(timeout);
            }

            Log($@"[ending] : {name}");
        }

        private void Log(string message)
        {
            Console.WriteLine($@"{DateTime.Now:O} - {message}");
        }
    }
}