using ActorsCP.TestActors;
using ActorsCP.Actors;

using NUnit.Framework;
using System.Threading.Tasks;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты набора акторов")]
    public sealed class ActorsSetTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(TestName = "10. Пустой набор акторов")]
        public Task EmptyTest()
            {
            var aset = new ActorsSet();
            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(0, aset.TotalCount);
            return Task.CompletedTask;
            }

        [Test]
        [TestCase(TestName = "11. 1 Pending актор")]
        public Task PendingTest_1()
            {
            var actor = base.NewPendingActor;
            var aset = new ActorsSet();
            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(0, aset.TotalCount);

            aset.Add(actor);
            Assert.AreEqual(1, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(1, aset.TotalCount);
            return Task.CompletedTask;
            }

        [Test]
        [TestCase(TestName = "12. 2 Pending актора")]
        public Task PendingTest_2()
            {
            var actor = base.NewPendingActor;
            var aset = new ActorsSet();
            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(0, aset.TotalCount);

            aset.Add(actor);
            Assert.AreEqual(1, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(1, aset.TotalCount);

            actor = base.NewPendingActor;

            aset.Add(actor);
            Assert.AreEqual(2, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(2, aset.TotalCount);
            return Task.CompletedTask;
            }

        [Test]
        [TestCase(TestName = "13. 1 runnning RunOnce актор")]
        public async Task RunningTest_1()
            {
            var actor = base.NewPendingActor;
            var aset = new ActorsSet();
            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(0, aset.TotalCount);

            await actor.RunAsync();
            aset.Add(actor);
            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(1, aset.CompletedCount);
            Assert.AreEqual(1, aset.TotalCount);
            }

        [Test]
        [TestCase(TestName = "14. 1 runnning & 1 Pending  актор")]
        public async Task RunningPendingTest_1()
            {
            var actor = base.NewPendingActor;
            var aset = new ActorsSet();
            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(0, aset.TotalCount);

            aset.Add(actor);
            Assert.AreEqual(1, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(1, aset.TotalCount);

            actor = base.NewPendingActor;
            await actor.RunAsync();
            aset.Add(actor);

            Assert.AreEqual(1, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(1, aset.CompletedCount);
            Assert.AreEqual(2, aset.TotalCount);
            }

        [Test]
        [TestCase(TestName = "15. 1 Pending->Run() актор")]
        public async Task Pending_Run_Test_1()
            {
            var actor = base.NewPendingActor;
            var aset = new ActorsSet();
            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(0, aset.TotalCount);

            aset.Add(actor);
            await actor.RunAsync();
            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(1, aset.CompletedCount);
            Assert.AreEqual(1, aset.TotalCount);
            }
        }
    }