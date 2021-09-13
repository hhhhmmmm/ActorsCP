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
        [TestCase(TestName = "11. Толпа из одного элемента")]
        public async Task SimpleActorCrowd1()
            {
            var crowd = new ActorsCrowd();
            crowd.SetCleanupAfterTermination(true);

            Assert.AreEqual(0, crowd.WaitingCount);
            Assert.AreEqual(0, crowd.RunningCount);
            Assert.AreEqual(0, crowd.CompletedCount);
            Assert.AreEqual(0, crowd.TotalCount);

            const int nCount = 1;

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