using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты ExternalObjects")]
    public class ExternalObjectsTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(TestName = "10. ExternalObjects")]
        public Task ExternalObjects_Tests()
            {
            var actor = NewPendingActor;
            var externalObjectsList = actor.ExternalObjects;
            Assert.IsNotNull(externalObjectsList);
            externalObjectsList.TryAdd("aaa", new WeakReference("aaa value"));
            Assert.AreEqual(1, externalObjectsList.Count);
            Assert.AreEqual(1, actor.ExternalObjects.Count);

            actor.ExternalObjects.TryAdd("bbb", new WeakReference("bbb value"));
            Assert.AreEqual(2, externalObjectsList.Count);
            Assert.AreEqual(2, actor.ExternalObjects.Count);

            actor.ExternalObjects.TryGetValue("bbb", out WeakReference wr);

            actor.ExternalObjects.Clear();
            Assert.AreEqual(0, externalObjectsList.Count);
            Assert.AreEqual(0, actor.ExternalObjects.Count);
            return Task.CompletedTask;
            }

        //
        } // end class ExternalObjectsTests
    } // end namespace ActorsCP.Unit.Test