using System.Threading.Tasks;

using ActorsCP.Actors.Events;
using ActorsCP.dotNET.ViewPorts;
using ActorsCP.ViewPorts;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты Tpl вьюпорта")]
    public class TplViewPortTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(true, TestName = "10. Простой Tpl вьюпорт - старт/стоп")]
        [TestCase(false, TestName = "10. Простой Queue вьюпорт - старт/стоп")]
        public void SimpleActorTest(bool useQueueForBuffering)
            {
            using (var viewport = new BufferedActorViewPortBase())
                {
                viewport.UseQueueForBuffering = useQueueForBuffering;
                viewport.Init();
                Assert.IsTrue(viewport.IsInitialized);
                viewport.Terminate();
                Assert.IsTrue(viewport.IsTerminated);

                viewport.ValidateStatistics();
                }
            }

        [Test]
        [TestCase(1, false, TestName = "20. 1 - Tpl вьюпорт")]
        [TestCase(1, true, TestName = "20. 1 - Queue вьюпорт")]
        [TestCase(10, false, TestName = "30. 10 - Tpl вьюпорт")]
        [TestCase(10, true, TestName = "30. 10 - Queue вьюпорт")]
        [TestCase(1000, false, TestName = "40. 1000 - Tpl вьюпорт")]
        [TestCase(1000, true, TestName = "40. 1000 - Queue вьюпорт")]
        [TestCase(10000, false, TestName = "50. 10000 - Tpl вьюпорт")]
        [TestCase(10000, true, TestName = "50. 10000 - Queue вьюпорт")]
        [TestCase(50000, false, TestName = "60. 50000 - Tpl вьюпорт")]
        [TestCase(50000, true, TestName = "60. 50000 - Queue вьюпорт")]
        [TestCase(75000, false, TestName = "70. 75000 - Tpl вьюпорт")]
        [TestCase(75000, true, TestName = "70. 75000 - Queue вьюпорт")]
        [TestCase(100000, false, TestName = "80. 100000 - Tpl вьюпорт")]
        [TestCase(100000, true, TestName = "80. 100000 - Queue вьюпорт")]
        [TestCase(300000, false, TestName = "90. 300000 - Tpl вьюпорт")]
        [TestCase(300000, true, TestName = "90. 300000 - Queue вьюпорт")]
        public void SimpleActorTest2(int N, bool useQueueForBuffering)
            {
            using (var viewport = new BufferedActorViewPortBase())
                {
                viewport.UseQueueForBuffering = useQueueForBuffering;
                viewport.Init();
                Assert.IsTrue(viewport.IsInitialized);

                var ea = new ActorEventArgs();
                for (int i = 0; i < N; i++)
                    {
                    var actor = base.TestActorsBuilder.NewSimpleActor;
                    var viewPortItem = new ViewPortItem(actor, ea);
                    viewport.Add(viewPortItem);
                    }

                viewport.Terminate();
                Assert.IsTrue(viewport.IsTerminated);

                var stat = viewport.СurrentExecutionStatistics;

                Assert.AreEqual(stat.BufferedAddedMessages, stat.BufferedProcessedMessages);
                Assert.AreEqual(N, stat.BufferedAddedMessages);
                Assert.AreEqual(N, stat.BufferedProcessedMessages);
                viewport.ValidateStatistics();
                }
            }
        } // end class TplViewPortTests
    } // end namespace ActorsCP.Unit.Test