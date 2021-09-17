using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.Tests.TestActors;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    /// <summary>
    ///
    /// </summary>
    [TestFixture]
    [Category("Тесты очереди")]
    public class ActorQueueTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(TestName = "10. Пустая очередь")]
        public Task EmptyQueue()
            {
            var queue = new ActorsQueue();
            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(0, queue.TotalCount);
            return Task.CompletedTask;
            }

        [Test]
        [TestCase(TestName = "11. Очередь из одного элемента")]
        public async Task SimpleActorQueue1()
            {
            var queue = new ActorsQueue();
            queue.SetCleanupAfterTermination(true);

            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(0, queue.TotalCount);

            const int nCount = 1;

            for (int i = 0; i < nCount; i++)
                {
                string name = string.Format("Объект {0}", i + 1);
                var actor = new SimpleActor(name);
                queue.Add(actor);
                }

            Assert.AreEqual(nCount, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(nCount, queue.TotalCount);

            await queue.RunAsync();

            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(nCount, queue.CompletedCount);
            Assert.AreEqual(nCount, queue.TotalCount);
            }

        [Test]
        [TestCase(1, true, TestName = "12. 1 T - Очередь CleanupAfterTermination нескольких элементов")]
        [TestCase(2, true, TestName = "12. 2 T- Очередь CleanupAfterTermination нескольких элементов")]
        [TestCase(10, true, TestName = "12. 10 T- Очередь CleanupAfterTermination нескольких элементов")]
        [TestCase(100, true, TestName = "12. 100 T - Очередь CleanupAfterTermination нескольких элементов")]
        [TestCase(1, false, TestName = "12. 1 F - Очередь CleanupAfterTermination нескольких элементов")]
        [TestCase(2, false, TestName = "12. 2 F - Очередь CleanupAfterTermination нескольких элементов")]
        [TestCase(10, false, TestName = "12. 10 F - Очередь CleanupAfterTermination нескольких элементов")]
        [TestCase(100, false, TestName = "12. 100 F - Очередь CleanupAfterTermination нескольких элементов")]
        public async Task SimpleActorQueueN(int nCount, bool cleanupAfterTermination)
            {
            var queue = new ActorsQueue();
            queue.SetCleanupAfterTermination(cleanupAfterTermination);
            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(0, queue.TotalCount);

            for (int i = 0; i < nCount; i++)
                {
                string name = string.Format("Объект {0}", i + 1);
                var actor = new SimpleActor(name);
                queue.Add(actor);
                }

            Assert.AreEqual(nCount, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(nCount, queue.TotalCount);

            await queue.RunAsync();

            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(nCount, queue.CompletedCount);
            Assert.AreEqual(nCount, queue.TotalCount);
            }

        [Test]
        [TestCase(1, 1, true, TestName = "13. T 1-1 Очередь RunOnlyOnce несколько раз")]
        [TestCase(1, 10, true, TestName = "13. T 1-10 Очередь RunOnlyOnce несколько раз")]
        // [TestCase(10, 1, true, TestName = "13. T 10-1 Очередь RunOnlyOnce несколько раз")]
        // [TestCase(10, 10, true, TestName = "13. T 10-10 Очередь RunOnlyOnce несколько раз")]
        [TestCase(1, 1, false, TestName = "13. F 1-1 Очередь RunOnlyOnce несколько раз")]
        [TestCase(1, 10, false, TestName = "13. F 1-10 Очередь RunOnlyOnce несколько раз")]
        //  [TestCase(10, 1, false, TestName = "13. F 10-1 Очередь RunOnlyOnce несколько раз")]
        //  [TestCase(10, 10, false, TestName = "13. F 10-10 Очередь RunOnlyOnce несколько раз")]
        public async Task SimpleActorQueueN_Times(int T, int E, bool runOnlyOnce)
            {
            var queue = new ActorsQueue();
            queue.SetRunOnlyOnce(runOnlyOnce);
            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(0, queue.TotalCount);

            for (int j = 0; j < T; j++)
                {
                for (int i = 0; i < E; i++)
                    {
                    string name = string.Format("Объект {0}-{1}", j + 1, i + 1);
                    var actor = new SimpleActor(name);
                    queue.Add(actor);
                    }

                Assert.AreEqual(E, queue.WaitingCount);
                Assert.AreEqual(0, queue.RunningCount);
                Assert.AreEqual(E * j, queue.CompletedCount);
                Assert.AreEqual(E * (j + 1), queue.TotalCount);

                await queue.RunAsync();
                }

            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(E * T, queue.CompletedCount);
            Assert.AreEqual(E * T, queue.TotalCount);
            }

        [Test]
        [TestCase(1, true, false, TestName = "14.1. 1TF смешанный набор акторов (FailureOnRun)")]
        [TestCase(2, true, true, TestName = "14.2. 2TT смешанный набор акторов (FailureOnRun)")]
        public async Task MixedQueueTimes(int nCount, bool expectedResult, bool anErrorOccurred)
            {
            var queue = new ActorsQueue();
            ActorBase actor = null;

            for (var i = 0; i < nCount; i++)
                {
                switch (i % 2)
                    {
                    case 0:
                        {
                        actor = new SimpleActor();
                        break;
                        }
                    case 1:
                        {
                        var _actor = new NoExceptionFailureActor();
                        _actor.FailureOnRun = true;
                        actor = _actor;
                        break;
                        }
                    } // end switch
                queue.Add(actor);
                }

            bool bres = await queue.RunAsync();
            Assert.AreEqual(expectedResult, bres);

            Assert.AreEqual(anErrorOccurred, actor.AnErrorOccurred);
            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(nCount, queue.CompletedCount);
            Assert.AreEqual(nCount, queue.TotalCount);
            }
        } // end class ActorQueueTests
    } // end namespace ActorsCP.Unit.Test