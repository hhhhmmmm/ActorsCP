using ActorsCP.Tests.TestActors;
using ActorsCP.Actors;

using NUnit.Framework;
using System.Threading.Tasks;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты набора объектов")]
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
        [TestCase(TestName = "10. Пустой набор объектов")]
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
        [TestCase(TestName = "11. 1 Pending объект")]
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
        [TestCase(TestName = "12. 2 Pending объект")]
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
        [TestCase(TestName = "13. 1 runnning RunOnce объект")]
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
        [TestCase(TestName = "14. 1 runnning & 1 Pending  объект")]
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
        [TestCase(TestName = "15. 1 Pending->Run() объект")]
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

        [Test]
        [TestCase(TestName = "16. Exception")]
        public async Task Exception_Test_1()
            {
            var exceptionActor = base.NewExceptionActor;
            exceptionActor.ExceptionOnRun = true;

            var aset = new ActorsSet();
            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(0, aset.CompletedCount);
            Assert.AreEqual(0, aset.TotalCount);

            aset.Add(exceptionActor);
            await exceptionActor.RunAsync();

            Assert.AreEqual(0, aset.WaitingCount);
            Assert.AreEqual(0, aset.RunningCount);
            Assert.AreEqual(1, aset.CompletedCount);
            Assert.AreEqual(1, aset.TotalCount);
            }
        }
    }