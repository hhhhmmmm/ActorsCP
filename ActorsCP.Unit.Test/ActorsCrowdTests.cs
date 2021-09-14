using System;
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
            crowd.SetMaxDegreeOfParallelism(null);

            Assert.AreEqual(0, crowd.WaitingCount);
            Assert.AreEqual(0, crowd.RunningCount);
            Assert.AreEqual(0, crowd.CompletedCount);
            Assert.AreEqual(0, crowd.TotalCount);
            return Task.CompletedTask;
            }

        [Test]
        [TestCase(1, true, TestName = "11. 1 T - Толпа из [1] элементов")]
        [TestCase(2, true, TestName = "11. 2 T - Толпа из [2] элементов")]
        [TestCase(5, true, TestName = "11. 5 T - Толпа из [5] элементов")]
        [TestCase(10, true, TestName = "11. 10 T - Толпа из [10] элементов")]
        [TestCase(1000, false, TestName = "11. 1000 F - Толпа из [1000] элементов")]
        [TestCase(1000, true, TestName = "11. 1000 T - Толпа из [1000] элементов")]
        [TestCase(10000, false, TestName = "11. 10000 F - Толпа из [10000] элементов")]
        [TestCase(10000, true, TestName = "11. 10000 T - Толпа из [10000] элементов")]
        [TestCase(20000, false, TestName = "11. 20000 F - Толпа из [20000] элементов")]
        [TestCase(20000, true, TestName = "11. 20000 T - Толпа из [20000] элементов")]
        [TestCase(50000, false, TestName = "11. 50000 F - Толпа из [50000] элементов")]
        [TestCase(50000, true, TestName = "11. 50000 T - Толпа из [50000] элементов")]
        [TestCase(100000, false, TestName = "11. 100000 F - Толпа из [100000] элементов")]
        public async Task SimpleActorCrowd1(int nCount, bool limitParallelelism)
            {
            var crowd = new ActorsCrowd();
            crowd.SetCleanupAfterTermination(true);

            if (limitParallelelism)
                {
                crowd.SetMaxDegreeOfParallelism(Environment.ProcessorCount);
                }
            else
                {
                crowd.SetMaxDegreeOfParallelism(null);
                }

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