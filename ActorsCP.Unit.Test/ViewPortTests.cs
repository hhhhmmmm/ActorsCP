using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.Options;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты вьюпорта")]
    public class ViewPortTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(TestName = "10. Простой объект и простой вьюпорт - TerminateAsync")]
        public async Task SimpleActorTest()
            {
            var actor = base.TestActorsBuilder.NewSimpleActor;
            var viewport = TestActorsBuilder.CreateViewPort();
            actor.BindEventsHandlers(viewport);
            Assert.AreEqual(1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(0, viewport._Counter_Actor_EventHandlersUnbound);

            await actor.TerminateAsync();
            Assert.AreEqual(1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(1, viewport._Counter_Actor_EventHandlersUnbound);
            }

        [Test]
        [TestCase(TestName = "20. Простой объект и простой вьюпорт - Dispose")]
        public Task SimpleActorTest2()
            {
            var actor = base.TestActorsBuilder.NewSimpleActor;
            var viewport = TestActorsBuilder.CreateViewPort();
            actor.BindEventsHandlers(viewport);
            Assert.AreEqual(1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(0, viewport._Counter_Actor_EventHandlersUnbound);

            actor.Dispose();

            Assert.AreEqual(1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(1, viewport._Counter_Actor_EventHandlersUnbound);
            return Task.CompletedTask;
            }

        [Test]
        [TestCase(TestName = "30. Простой объект и простой вьюпорт - TerminateAsync + Dispose")]
        public async Task SimpleActorTest3()
            {
            var actor = base.TestActorsBuilder.NewSimpleActor;
            var viewport = TestActorsBuilder.CreateViewPort();
            actor.BindEventsHandlers(viewport);
            Assert.AreEqual(1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(0, viewport._Counter_Actor_EventHandlersUnbound);

            await actor.TerminateAsync();
            actor.Dispose();

            Assert.AreEqual(1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(1, viewport._Counter_Actor_EventHandlersUnbound);
            }

        [Test]
        [TestCase(1, TestName = "40. 1.  Очередь Pending и простой вьюпорт")]
        [TestCase(2, TestName = "40. 2.  Очередь Pending и простой вьюпорт")]
        [TestCase(30, TestName = "40. 30. Очередь Pending и простой вьюпорт")]
        public async Task SimpleActorTest4(int nActorCount)
            {
            var queue = new ActorsQueue();
            var actors = base.TestActorsBuilder.CreateListOfActors('S', nActorCount);
            queue.Add(actors);
            var viewport = TestActorsBuilder.CreateViewPort();
            queue.BindEventsHandlers(viewport);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(0, viewport._Counter_Actor_EventHandlersUnbound);

            await queue.TerminateAsync();
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersUnbound);

            queue.Dispose();

            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersUnbound);
            }

        [Test]
        [TestCase(1, TestName = "50. 1.  Очередь Run и простой вьюпорт")]
        [TestCase(2, TestName = "50. 2.  Очередь Run и простой вьюпорт")]
        [TestCase(30, TestName = "50. 30. Очередь Run и простой вьюпорт")]
        public async Task SimpleActorTest5(int nActorCount)
            {
            var queue = new ActorsQueue();
            var actors = base.TestActorsBuilder.CreateListOfActors('S', nActorCount);
            queue.Add(actors);
            var viewport = TestActorsBuilder.CreateViewPort();
            queue.BindEventsHandlers(viewport);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(0, viewport._Counter_Actor_EventHandlersUnbound);

            await queue.RunAsync();
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersUnbound);

            queue.Dispose();

            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersUnbound);
            }

        [Test]
        [TestCase(1, TestName = "60. 1.  Толпа Pending и простой вьюпорт")]
        [TestCase(2, TestName = "60. 2.  Толпа Pending и простой вьюпорт")]
        [TestCase(30, TestName = "60. 30. Толпа Pending и простой вьюпорт")]
        public async Task SimpleActorTest6(int nActorCount)
            {
            var crowd = new ActorsCrowd();
            var actors = base.TestActorsBuilder.CreateListOfActors('S', nActorCount);
            crowd.Add(actors);
            var viewport = TestActorsBuilder.CreateViewPort();
            crowd.BindEventsHandlers(viewport);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(0, viewport._Counter_Actor_EventHandlersUnbound);

            await crowd.TerminateAsync();
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersUnbound);

            crowd.Dispose();

            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersUnbound);
            }

        [Test]
        [TestCase(1, TestName = "70. 1.  Толпа Pending и простой вьюпорт")]
        [TestCase(2, TestName = "70. 2.  Толпа Pending и простой вьюпорт")]
        [TestCase(30, TestName = "70. 30. Толпа Pending и простой вьюпорт")]
        public async Task SimpleActorTest7(int nActorCount)
            {
            var crowd = new ActorsCrowd();
            var actors = base.TestActorsBuilder.CreateListOfActors('S', nActorCount);
            crowd.Add(actors);
            var viewport = TestActorsBuilder.CreateViewPort();
            crowd.BindEventsHandlers(viewport);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(0, viewport._Counter_Actor_EventHandlersUnbound);

            await crowd.RunAsync();
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersUnbound);

            crowd.Dispose();

            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersBound);
            Assert.AreEqual(nActorCount + 1, viewport._Counter_Actor_EventHandlersUnbound);
            }
        } // end class ViewPortTests
    } // end namespace ActorsCP.Unit.Test