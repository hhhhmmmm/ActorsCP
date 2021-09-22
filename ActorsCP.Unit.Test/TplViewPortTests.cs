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
        [TestCase(TestName = "10. Простой Tpl вьюпорт - старт/стоп")]
        public async Task SimpleActorTest()
            {
            using (var viewport = new BufferedActorViewPortBase())
                {
                viewport.InitTplDataFlow();
                Assert.IsTrue(viewport.TplDataFlowInitialized);
                await viewport.TerminateTplDataFlowAsync();
                Assert.IsTrue(viewport.TplDataFlowTerminated);

                viewport.ValidateStatistics();
                }
            }

        [Test]
        [TestCase(1, TestName = "20. 1 - Tpl вьюпорт")]
        [TestCase(10, TestName = "30. 10 - Tpl вьюпорт")]
        [TestCase(1000, TestName = "40. 1000 - Tpl вьюпорт")]
        [TestCase(10000, TestName = "50. 10000 - Tpl вьюпорт")]
        [TestCase(50000, TestName = "60. 50000 - Tpl вьюпорт")]
        [TestCase(75000, TestName = "70. 75000 - Tpl вьюпорт")]
        [TestCase(100000, TestName = "80. 100000 - Tpl вьюпорт")]
        [TestCase(300000, TestName = "90. 300000 - Tpl вьюпорт")]
        public async Task SimpleActorTest2(int N)
            {
            using (var viewport = new BufferedActorViewPortBase())
                {
                viewport.InitTplDataFlow();
                Assert.IsTrue(viewport.TplDataFlowInitialized);

                var ea = new ActorEventArgs();
                for (int i = 0; i < N; i++)
                    {
                    var actor = base.TestActorsBuilder.NewSimpleActor;
                    var viewPortItem = new ViewPortItem(actor, ea);
                    await viewport.TplDataFlowAddDataAsync(viewPortItem);
                    }

                await viewport.TerminateTplDataFlowAsync();
                Assert.IsTrue(viewport.TplDataFlowTerminated);

                var stat = viewport.СurrentExecutionStatistics;

                Assert.AreEqual(stat.TplAddedMessages, stat.TplProcessedMessages);
                Assert.AreEqual(N, stat.TplAddedMessages);
                Assert.AreEqual(N, stat.TplProcessedMessages);
                viewport.ValidateStatistics();
                }
            }
        } // end class TplViewPortTests
    } // end namespace ActorsCP.Unit.Test