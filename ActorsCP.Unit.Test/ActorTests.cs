using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.Tests.TestActors;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты объекта")]
    public sealed class ActorTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(TestName = "10. Тесты состояния пустого объекта")]
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
            var exStart = new ExceptionActor
                {
                ExceptionOnStart = true
                };
            bres = await exStart.StartAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exStart.State, ActorState.Terminated);
            Assert.IsTrue(exStart.AnErrorOccurred);

            // стоп
            var exStop = new ExceptionActor
                {
                ExceptionOnStop = true
                };
            bres = await exStop.StartAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(exStop.State, ActorState.Started);
            Assert.IsFalse(exStop.AnErrorOccurred);

            bres = await exStop.StopAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exStop.State, ActorState.Terminated);
            Assert.IsTrue(exStop.AnErrorOccurred);

            // run
            var exRun = new ExceptionActor
                {
                ExceptionOnRun = true
                };
            bres = await exRun.RunAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exRun.State, ActorState.Terminated);
            Assert.IsTrue(exRun.AnErrorOccurred);

            // Cleanup
            var exCleanup = new ExceptionActor();
            exCleanup.ExceptionOnRunCleanupBeforeTerminationAsync = true;
            bres = await exCleanup.RunAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exCleanup.State, ActorState.Terminated);
            Assert.IsTrue(exCleanup.AnErrorOccurred);
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

        [Test]
        [TestCase(TestName = "50. Тесты отмены без запуска")]
        public async Task CancellationTests_50()
            {
            var a = new ExceptionActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsFalse(a.IsCanceled);
            await a.CancelAsync();
            Assert.IsTrue(a.IsCanceled);
            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
            }

        [Test]
        [TestCase(TestName = "51. Тесты отмены с запуском")]
        public async Task CancellationTests_51()
            {
            bool bres;
            var a = new ExceptionActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsFalse(a.IsCanceled);

            bres = await a.StartAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Started, a.State);

            await a.CancelAsync();
            Assert.IsTrue(a.IsCanceled);

            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
            }

        [Test]
        [TestCase(TestName = "52. Тесты отмены с запуском")]
        public async Task CancellationTests_52()
            {
            bool bres;
            var a = new ExceptionActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsFalse(a.IsCanceled);

            bres = await a.StartAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Started, a.State);

            bres = await a.StopAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Stopped, a.State);

            await a.CancelAsync();
            Assert.IsTrue(a.IsCanceled);

            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
            }

        [Test]
        [TestCase(TestName = "55. Тесты отмены с запуском")]
        public async Task CancellationTests_55()
            {
            bool bres;
            var a = new ExceptionActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsFalse(a.IsCanceled);

            bres = await a.RunAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения

            await a.CancelAsync();
            Assert.IsTrue(a.IsCanceled);

            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
            }

        [Test]
        [TestCase(TestName = "60. Перегрузки InternalXXX")]
        public async Task InternalXxx()
            {
            SimpleActor actor;
            using (var a = new SimpleActor())
                {
                actor = a;
                await a.RunAsync();
                Assert.IsTrue(a.InternalInitLogger_Called);
                Assert.IsTrue(a.InternalStartAsync_Called);
                Assert.IsTrue(a.InternalStopAsync_Called);
                Assert.IsTrue(a.InternalRunAsync_Called);
                Assert.IsTrue(a.InternalRunCleanupBeforeTerminationAsync_Called);
                }
            Assert.IsTrue(actor.DisposeManagedResources_Called);
            Assert.IsTrue(actor.DisposeUnmanagedResources_Called);
            }
        }
    }