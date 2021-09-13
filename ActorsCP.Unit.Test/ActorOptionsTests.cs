using System.Threading.Tasks;
using ActorsCP.Actors;
using ActorsCP.Options;
using ActorsCP.TestActors;
using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты опций актора ActorOptions")]
    public sealed class ActorOptionsTests : TestBase
        {
        private const string dogKey = "ключСобака";
        private const string catKey = "ключКот";

        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            var gao = GlobalActorOptions.GetInstance();
            gao.AddOrUpdate(dogKey, "Жучка");
            gao.AddOrUpdate(catKey, "Мурзик");
            }

        [Test]
        [TestCase(TestName = "100. Тесты ActorOptions")]
        public async Task StateTests()
            {
            var ao = new ActorOptions();
            ao.AddOrUpdate("s", "ssss");
            ao.AddOrUpdate("s", "zzzz");

            object result;
            ao.Get("s", out result);
            ao.Get(dogKey, out result);
            }
        }
    }