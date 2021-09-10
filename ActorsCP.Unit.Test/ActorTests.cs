using ActorsCP.TestActors;
using ActorsCP.Actors;

using NUnit.Framework;
using System.Threading.Tasks;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    // [Description("Тесты актора")]
    [Category("Тесты актора")]
    public class ActorTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(TestName = "10. Тесты состояния актора пустого актора")]
        public async Task StateTests()
            {
            bool bres;
            var a = new SimpleActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsNotNull(a.Name);

            // старт
            bres = await a.StartAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Started, a.State);

            // стоп
            bres = await a.StopAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Stopped, a.State);

            // старт
            bres = await a.StartAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Started, a.State);

            // выполнение
            bres = await a.RunAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
            }

        [Test]
        [TestCase(TestName = "20. Исключения при перегрузках")]
        public async Task ExceptionTests()
            {
            bool bres;
            // старт
            var exStart = new ExceptionActor();
            exStart.ExceptionOnStart = true;
            bres = await exStart.StartAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exStart.State, ActorState.Terminated);

            // стоп
            var exStop = new ExceptionActor();
            exStop.ExceptionOnStop = true;
            bres = await exStop.StartAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(exStop.State, ActorState.Started);

            bres = await exStop.StopAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exStop.State, ActorState.Terminated);

            // run
            var exRun = new ExceptionActor();
            exRun.ExceptionOnRun = true;
            bres = await exRun.RunAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exRun.State, ActorState.Terminated);

            // Cleanup
            var exCleanup = new ExceptionActor();
            exCleanup.ExceptionOnRunCleanupBeforeTerminationAsync = true;
            bres = await exCleanup.RunAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exCleanup.State, ActorState.Terminated);
            }

        [Test]
        [TestCase(TestName = "30. Тесты однократного запуска")]
        public async Task RunOnceTests()
            {
            bool bres;
            var a = new SimpleActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsNotNull(a.Name);
            Assert.IsTrue(a.RunOnlyOnce);

            // выполнение
            bres = await a.RunAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения

            bres = await a.RunAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
            }

        [Test]
        [TestCase(TestName = "40. Тесты многократного запуска")]
        public async Task RunManyTests()
            {
            bool bres;
            var a = new SimpleActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsNotNull(a.Name);
            Assert.IsTrue(a.RunOnlyOnce);
            a.SetRunOnlyOnce(false);
            Assert.IsFalse(a.RunOnlyOnce);

            // выполнение
            bres = await a.RunAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Started, a.State); // после завершения

            bres = await a.RunAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Started, a.State); // после завершения
            }
        }
    }