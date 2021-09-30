using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;
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
            if (a.RunOnlyOnce)
                {
                Assert.AreEqual(ActorState.Terminated, a.State);
                }
            else
                {
                Assert.AreEqual(ActorState.Stopped, a.State);
                }

            // старт
            if (!a.RunOnlyOnce)
                {
                bres = await a.StartAsync();
                Assert.IsTrue(bres);
                Assert.AreEqual(ActorState.Started, a.State);
                }
            else
                {
                bres = await a.StartAsync();
                Assert.IsFalse(bres);
                Assert.AreEqual(ActorState.Terminated, a.State);
                }

            // выполнение
            if (!a.RunOnlyOnce)
                {
                bres = await a.RunAsync();
                Assert.IsTrue(bres);
                Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
                }
            else
                {
                bres = await a.RunAsync();
                Assert.IsFalse(bres);
                Assert.AreEqual(ActorState.Terminated, a.State);
                }
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
            try
                {
                exCleanup.ExceptionOnRunCleanupBeforeTerminationAsync = true;
                bres = await exCleanup.RunAsync();
                Assert.IsFalse(bres);
                Assert.AreEqual(exCleanup.State, ActorState.Terminated);
                Assert.IsTrue(exCleanup.AnErrorOccurred);
                }
            catch (Exception)
                {
                }
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
            Assert.IsFalse(a.IsCancellationRequested);
            await a.CancelAsync();
            Assert.IsTrue(a.IsCancellationRequested);
            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
            }

        [Test]
        [TestCase(TestName = "51. Тесты отмены с запуском")]
        public async Task CancellationTests_51()
            {
            bool bres;
            var a = new ExceptionActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsFalse(a.IsCancellationRequested);

            bres = await a.StartAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Started, a.State);

            await a.CancelAsync();
            Assert.IsTrue(a.IsCancellationRequested);

            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
            }

        [Test]
        [TestCase(TestName = "52. Тесты отмены с запуском")]
        public async Task CancellationTests_52()
            {
            bool bres;
            var a = new ExceptionActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsFalse(a.IsCancellationRequested);

            bres = await a.StartAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Started, a.State);

            bres = await a.StopAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Terminated, a.State);

            await a.CancelAsync();
            Assert.IsTrue(a.IsCancellationRequested);

            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения
            }

        [Test]
        [TestCase(TestName = "55. Тесты отмены с запуском")]
        public async Task CancellationTests_55()
            {
            bool bres;
            var a = new ExceptionActor();
            Assert.AreEqual(ActorState.Pending, a.State);
            Assert.IsFalse(a.IsCancellationRequested);

            bres = await a.RunAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(ActorState.Terminated, a.State); // после завершения

            await a.CancelAsync();
            Assert.IsTrue(a.IsCancellationRequested);

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
                Assert.IsTrue(a.InternalStartAsync_Called);
                Assert.IsTrue(a.InternalStopAsync_Called);
                Assert.IsTrue(a.InternalRunAsync_Called);
                Assert.IsTrue(a.InternalRunCleanupBeforeTerminationAsync_Called);
                }
            Assert.IsTrue(actor.DisposeManagedResources_Called);
            Assert.IsTrue(actor.DisposeUnmanagedResources_Called);
            }

        [Test]
        [TestCase(1, TestName = "70. 1 - порядок событий")]
        [TestCase(10, TestName = "71. 10 - порядок событий")]
        [TestCase(100, TestName = "72. 100 - порядок событий")]
        [TestCase(1000, TestName = "73. 1000 - порядок событий")]
        [TestCase(1000, TestName = "74. 10000 - порядок событий")]
        public async Task Events1(int N)
            {
            for (int i = 0; i < N; i++)
                {
                var list = new List<ActorStateChangedEventArgs>();

                using (var actor = new SimpleActor())
                    {
                    actor.StateChangedEvents += (sender, e) => { list.Add(e); };
                    await actor.RunAsync();
                    }

                Assert.IsTrue(list.Count == 4);
                Assert.AreEqual(list[0], ActorStates.ActorStateChangedStarted);
                Assert.AreEqual(list[1], ActorStates.ActorStateChangedRunning);
                Assert.AreEqual(list[2], ActorStates.ActorStateChangedStopped);
                Assert.AreEqual(list[3], ActorStates.ActorStateChangedTerminated);
                }
            }
        }
    }