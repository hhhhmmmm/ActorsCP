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
        [TestCase(TestName = "11. Простая очередь")]
        public async Task SimpleActorQueue()
            {
            var queue = new ActorsQueue();
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
        } // end class ActorQueueTests
    } // end namespace ActorsCP.Unit.Test