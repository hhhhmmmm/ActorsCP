using System.Threading.Tasks;

using ActorsCP.Actors;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты токена отмены")]
    public class CancellationTokenTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(TestName = "Одиночный токен - Pending")]
        public async Task CancelAsync1()
            {
            var actor = base.NewPendingActor;

            Assert.IsFalse(actor.IsCancellationRequested);
            await actor.CancelAsync();
            Assert.IsTrue(actor.IsCancellationRequested);
            }

        [Test]
        [TestCase(TestName = "Одиночный токен - Completed")]
        public async Task CancelAsync2()
            {
            var actor = base.NewPendingActor;

            Assert.IsFalse(actor.IsCancellationRequested);
            await actor.RunAsync();
            await actor.CancelAsync();
            Assert.IsTrue(actor.IsCancellationRequested);
            }

        [Test]
        [TestCase(0, TestName = "Очередь - 0 - Pending")]
        [TestCase(1, TestName = "Очередь - 1 - Pending")]
        [TestCase(5, TestName = "Очередь - 5 - Pending")]
        public async Task CancelAsyncQueuePending(int nCount)
            {
            var queue = new ActorsQueue();
            Assert.IsFalse(queue.IsCancellationRequested);

            var actors = new ActorBase[nCount];

            for (int i = 0; i < nCount; i++)
                {
                var actor = base.NewPendingActor;
                actors[i] = actor;
                Assert.IsFalse(actor.IsCancellationRequested);
                queue.Add(actor);
                }

            await queue.CancelAsync();

            Assert.IsTrue(queue.IsCancellationRequested);

            for (int i = 0; i < nCount; i++)
                {
                var actor = actors[i];
                Assert.IsTrue(actor.IsCancellationRequested);
                }
            }

        [Test]
        [TestCase(0, TestName = "Толпа - 0 - Pending")]
        [TestCase(1, TestName = "Толпа - 1 - Pending")]
        [TestCase(5, TestName = "Толпа - 5 - Pending")]
        public async Task CancelAsyncCrowdPending(int nCount)
            {
            var crowd = new ActorsCrowd();
            Assert.IsFalse(crowd.IsCancellationRequested);

            var actors = new ActorBase[nCount];

            for (int i = 0; i < nCount; i++)
                {
                var actor = base.NewPendingActor;
                actors[i] = actor;
                Assert.IsFalse(actor.IsCancellationRequested);
                crowd.Add(actor);
                }

            await crowd.CancelAsync();

            Assert.IsTrue(crowd.IsCancellationRequested);

            for (int i = 0; i < nCount; i++)
                {
                var actor = actors[i];
                Assert.IsTrue(actor.IsCancellationRequested);
                }
            }

        [Test]
        [TestCase(0, TestName = "Очередь - 0 - Completed")]
        [TestCase(1, TestName = "Очередь - 1 - Completed")]
        [TestCase(5, TestName = "Очередь - 5 - Completed")]
        public async Task CancelAsyncQueueCompleted(int nCount)
            {
            var queue = new ActorsQueue();
            queue.SetCleanupAfterTermination(false);
            Assert.IsFalse(queue.IsCancellationRequested);

            ActorBase[] actors = new ActorBase[nCount];

            for (int i = 0; i < nCount; i++)
                {
                var actor = base.NewPendingActor;
                actors[i] = actor;
                Assert.IsFalse(actor.IsCancellationRequested);
                queue.Add(actor);
                }

            await queue.RunAsync();
            await queue.CancelAsync();

            Assert.IsTrue(queue.IsCancellationRequested);

            for (int i = 0; i < nCount; i++)
                {
                var actor = actors[i];
                Assert.IsTrue(actor.IsCancellationRequested);
                }
            }

        [Test]
        [TestCase(0, TestName = "Толпа - 0 - Completed")]
        [TestCase(1, TestName = "Толпа - 1 - Completed")]
        [TestCase(5, TestName = "Толпа - 5 - Completed")]
        public async Task CancelAsyncCrowdCompleted(int nCount)
            {
            var crowd = new ActorsCrowd();
            crowd.SetCleanupAfterTermination(false);
            Assert.IsFalse(crowd.IsCancellationRequested);

            ActorBase[] actors = new ActorBase[nCount];

            for (int i = 0; i < nCount; i++)
                {
                var actor = base.NewPendingActor;
                actors[i] = actor;
                Assert.IsFalse(actor.IsCancellationRequested);
                crowd.Add(actor);
                }

            await crowd.RunAsync();
            await crowd.CancelAsync();

            Assert.IsTrue(crowd.IsCancellationRequested);

            for (int i = 0; i < nCount; i++)
                {
                var actor = actors[i];
                Assert.IsTrue(actor.IsCancellationRequested);
                }
            }

        //
        } // end class CancellationTokenTests
    } // end namespace ActorsCP.Unit.Test