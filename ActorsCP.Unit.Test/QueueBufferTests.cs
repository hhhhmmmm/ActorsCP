using System;
using System.Threading;
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
            var debugOptions = GlobalActorDebugOptions.GetInstance();
            debugOptions.AddOrUpdate(ActorDebugKeywords.QueueBufferT_Debug, true);
            }

        [Test]
        [TestCase(1, TestName = "1 - тест добавления и обработки")]
        [TestCase(2, TestName = "2 - тест добавления и обработки")]
        [TestCase(3, TestName = "3 - тест добавления и обработки")]
        [TestCase(10, TestName = "10 - тест добавления и обработки")]
        [TestCase(1000, TestName = "1000 - тест добавления и обработки")]
        [TestCase(10000, TestName = "10000 - тест добавления и обработки")]
        [TestCase(100000, TestName = "100000 - тест добавления и обработки")]
        [TestCase(1_000_000, TestName = "1000000 - тест добавления и обработки")]
        public async Task StateTests(int N)
            {
            bool debugQueueBufferT;
            var debugOptions = GlobalActorDebugOptions.GetInstance();
            debugOptions.GetBool(ActorDebugKeywords.QueueBufferT_Debug, out debugQueueBufferT);
            Assert.IsTrue(debugQueueBufferT);

            int ProcessedMessages = 0;

            Action<object> messageHandler = (object o) => { ProcessedMessages++; };

            var queue = QueueBufferT<object>.Create(messageHandler);

            for (int i = 0; i < N; i++)
                {
                var obj = i;
                queue.Add(obj);
                }

            await queue.WaitAsync();
            await queue.TerminateAsync();

            Assert.IsTrue(queue.IsTerminated);

            var statistics = queue.Statistics;
            Assert.AreEqual(statistics.AddedMessages, N);
            Assert.AreEqual(ProcessedMessages, N);
            }

        [Test]
        [TestCase(TestName = "S1 - семафор")]
        public void SemaphoreTests()
            {
            var semaphore = new SemaphoreSlim(0, int.MaxValue);
            Assert.AreEqual(0, semaphore.CurrentCount);
            semaphore.Release(); // каждый Release увеличивает счетчик
            Assert.AreEqual(1, semaphore.CurrentCount);
            semaphore.Release();
            Assert.AreEqual(2, semaphore.CurrentCount);
            semaphore.Release();
            Assert.AreEqual(3, semaphore.CurrentCount);
            }

        [Test]
        [TestCase(TestName = "S2 - семафор")]
        public void SemaphoreTests2()
            {
            var semaphore = new SemaphoreSlim(3, int.MaxValue);
            Assert.AreEqual(3, semaphore.CurrentCount);
            semaphore.Release(); // 3 + 1
            Assert.AreEqual(4, semaphore.CurrentCount);
            semaphore.Release(); // 4 + 1
            Assert.AreEqual(5, semaphore.CurrentCount);
            semaphore.Release(); // 5 + 1
            Assert.AreEqual(6, semaphore.CurrentCount);
            }

        [Test]
        [TestCase(TestName = "S3 - семафор")]
        public void SemaphoreTests3()
            {
            bool bres;
            var semaphore = new SemaphoreSlim(3, int.MaxValue);
            Assert.AreEqual(3, semaphore.CurrentCount);

            bres = semaphore.Wait(100); // 3 - 1 = 2
            Assert.AreEqual(2, semaphore.CurrentCount);
            Assert.IsTrue(bres);

            bres = semaphore.Wait(100); // 2 - 1 = 1
            Assert.AreEqual(1, semaphore.CurrentCount);
            Assert.IsTrue(bres);

            bres = semaphore.Wait(100); // 1 - 1 = 0
            Assert.AreEqual(0, semaphore.CurrentCount);
            Assert.IsTrue(bres);

            bres = semaphore.Wait(100); // 0 - 1 = 0
            Assert.AreEqual(0, semaphore.CurrentCount);
            Assert.IsFalse(bres);

            // semaphore.Wait(); // бесконечное ожидание
            }

        [Test]
        [TestCase(TestName = "S4 - семафор")]
        public void SemaphoreTests4()
            {
            bool bres;
            var terminatingEvent = new ManualResetEventSlim();
            var semaphore = new SemaphoreSlim(3, int.MaxValue);
            Assert.AreEqual(3, semaphore.CurrentCount);

            bres = semaphore.Wait(100); // 3 - 1 = 2
            Assert.AreEqual(2, semaphore.CurrentCount);
            Assert.IsTrue(bres);

            var waitHandles = new WaitHandle[2];
            waitHandles[0] = terminatingEvent.WaitHandle;
            waitHandles[1] = semaphore.AvailableWaitHandle;

            int waitResult = WaitHandle.WaitAny(waitHandles); // WaitHandle.WaitAny не уменьшает счетчик светофора!!!
            Assert.AreEqual(2, semaphore.CurrentCount);
            }
        } // end class QueueBufferTests
    } // end namespace ActorsCP.Unit.Test