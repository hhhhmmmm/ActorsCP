using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.TestActors;

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
        [TestCase(TestName = "11. Простая очередь из одного элемента")]
        public async Task SimpleActorQueue1()
            {
            var queue = new ActorsQueue();
            queue.SetCleanupAfterTermination(true);

            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(0, queue.TotalCount);

            const int N = 1;

            for (int i = 0; i < N; i++)
                {
                string name = string.Format("Объект {0}", i + 1);
                var actor = new SimpleActor(name);
                queue.Add(actor);
                }

            Assert.AreEqual(N, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(N, queue.TotalCount);

            await queue.RunAsync();

            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(N, queue.CompletedCount);
            Assert.AreEqual(N, queue.TotalCount);
            }

        [Test]
        [TestCase(TestName = "11. Простая очередь из нескольких элементов")]
        public async Task SimpleActorQueueN()
            {
            var queue = new ActorsQueue();
            queue.SetCleanupAfterTermination(true);
            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(0, queue.TotalCount);

            const int N = 10;

            for (int i = 0; i < N; i++)
                {
                string name = string.Format("Объект {0}", i + 1);
                var actor = new SimpleActor(name);
                queue.Add(actor);
                }

            Assert.AreEqual(N, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(N, queue.TotalCount);

            await queue.RunAsync();

            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(N, queue.CompletedCount);
            Assert.AreEqual(N, queue.TotalCount);
            }

        [Test]
        [TestCase(TestName = "11. Очередь запускаемая несколько раз")]
        public async Task SimpleActorQueueN_Times()
            {
            var queue = new ActorsQueue();
            queue.SetRunOnlyOnce(false);
            Assert.AreEqual(0, queue.WaitingCount);
            Assert.AreEqual(0, queue.RunningCount);
            Assert.AreEqual(0, queue.CompletedCount);
            Assert.AreEqual(0, queue.TotalCount);

            const int T = 10;
            const int E = 10;

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
        } // end class ActorQueueTests
    } // end namespace ActorsCP.Unit.Test