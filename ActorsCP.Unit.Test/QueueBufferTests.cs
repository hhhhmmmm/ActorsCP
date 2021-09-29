using System;
using System.Threading.Tasks;

using ActorsCP.Helpers;
using ActorsCP.Options;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты QueueBufferT")]
    public sealed class QueueBufferTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(1, TestName = "1 - тест добавления и обработки")]
        [TestCase(10, TestName = "10 - тест добавления и обработки")]
        [TestCase(1000, TestName = "1000 - тест добавления и обработки")]
        [TestCase(10000, TestName = "10000 - тест добавления и обработки")]
        [TestCase(100000, TestName = "100000 - тест добавления и обработки")]
        [TestCase(1_000_000, TestName = "1000000 - тест добавления и обработки")]
        public async Task StateTests(int N)
            {
            int ProcessedMessages = 0;

            Action<object> messageHandler = (object o) => { ProcessedMessages++; };

            var queue = new QueueBufferT<object>(messageHandler);

            for (int i = 0; i < N; i++)
                {
                var obj = i;
                queue.Add(obj);
                }

            Assert.IsFalse(queue.IsTerminating);

            await queue.WaitAsync();
            bool IsTerminating = queue.IsTerminating;
            await queue.TerminateAsync();

            Assert.IsFalse(queue.IsTerminating);
            Assert.IsTrue(queue.IsTerminated);

            var statistics = queue.Statistics;
            Assert.AreEqual(statistics.AddedMessages, N);
            Assert.AreEqual(ProcessedMessages, N);
            }
        } // end class QueueBufferTests
    } // end namespace ActorsCP.Unit.Test