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
        public async Task ExternalObjects_Tests()
            {
            var actor = NewPendingActor;
            var externalObjectsList = actor.ExternalObjects;
            Assert.IsNotNull(externalObjectsList);
            externalObjectsList.Add(new WeakReference("aaa"));
            Assert.AreEqual(1, externalObjectsList.Count);
            Assert.AreEqual(1, actor.ExternalObjects.Count);

            actor.ExternalObjects.Add(new WeakReference("bbb"));
            Assert.AreEqual(2, externalObjectsList.Count);
            Assert.AreEqual(2, actor.ExternalObjects.Count);
            }

        //
        } // end class ExternalObjectsTests
    } // end namespace ActorsCP.Unit.Test