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
    [Category("Тесты толпы")]
    public sealed class ActorsCrowdTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(TestName = "10. Пустая толпа")]
        public Task EmptyCrowd()
            {
            var crowd = new ActorsCrowd();
            Assert.AreEqual(0, crowd.WaitingCount);
            Assert.AreEqual(0, crowd.RunningCount);
            Assert.AreEqual(0, crowd.CompletedCount);
            Assert.AreEqual(0, crowd.TotalCount);
            return Task.CompletedTask;
            }

        [Test]
        [TestCase(1, TestName = "11.  1 - Толпа из [1] элементов")]
        [TestCase(2, TestName = "11.  2 - Толпа из [2] элементов")]
        [TestCase(5, TestName = "11.  5 - Толпа из [5] элементов")]
        [TestCase(10, TestName = "11. 10 - Толпа из [10] элементов")]
        [TestCase(1000, TestName = "11. 1000 - Толпа из [1000] элементов")]
        [TestCase(100000, TestName = "11. 10000 - Толпа из [100000] элементов")]
        public async Task SimpleActorCrowd1(int nCount)
            {
            var crowd = new ActorsCrowd();
            crowd.SetCleanupAfterTermination(true);

            Assert.AreEqual(0, crowd.WaitingCount);
            Assert.AreEqual(0, crowd.RunningCount);
            Assert.AreEqual(0, crowd.CompletedCount);
            Assert.AreEqual(0, crowd.TotalCount);

            for (int i = 0; i < nCount; i++)
                {
                string name = string.Format("Объект {0}", i + 1);
                var actor = new SimpleActor(name);
                crowd.Add(actor);
                }

            Assert.AreEqual(nCount, crowd.WaitingCount);
            Assert.AreEqual(0, crowd.RunningCount);
            Assert.AreEqual(0, crowd.CompletedCount);
            Assert.AreEqual(nCount, crowd.TotalCount);

            await crowd.RunAsync();

            Assert.AreEqual(0, crowd.WaitingCount);
            Assert.AreEqual(0, crowd.RunningCount);
            Assert.AreEqual(nCount, crowd.CompletedCount);
            Assert.AreEqual(nCount, crowd.TotalCount);
            }
        }
    }