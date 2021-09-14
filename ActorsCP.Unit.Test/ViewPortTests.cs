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
        [TestCase(1, TestName = "40. 1. Очередь и простой вьюпорт")]
        [TestCase(2, TestName = "40. 2. Очередь и простой вьюпорт")]
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
        } // end class ViewPortTests
    } // end namespace ActorsCP.Unit.Test