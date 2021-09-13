using System.Threading.Tasks;

using ActorsCP.Options;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    [Category("Тесты опций актора ActorOptions")]
    public sealed class ActorOptionsTests : TestBase
        {
        private const string sKey = "_sKey_";
        private const string dogKey = "ключСобака";
        private const string catKey = "ключКот";
        private const string boolKeyFalse = "boolKeyFalse";
        private const string boolKeyTrue = "boolKeyTrue";

        private const string dog = "Жучка";
        private const string cat = "Мурзик";

        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            var gao = GlobalActorOptions.GetInstance();
            gao.AddOrUpdate(dogKey, dog);
            gao.AddOrUpdate(catKey, cat);
            }

        [Test]
        [TestCase(TestName = "ActorOptions Get()")]
        public Task StateTests()
            {
            var ao = new ActorOptions();
            return InternalStateTests(ao);
            }

        private Task InternalStateTests(IActorOptions ao)
            {
            bool bres;
            object result;
            const string svalue1 = "111111111111111111";
            const string svalue2 = "222222222222222222";

            ao.AddOrUpdate(sKey, svalue1);
            bres = ao.Get(sKey, out result);
            Assert.IsTrue(bres);
            Assert.AreEqual(svalue1, result);

            ao.AddOrUpdate(sKey, svalue2);
            bres = ao.Get(sKey, out result);
            Assert.IsTrue(bres);
            Assert.AreEqual(svalue2, result);

            bres = ao.Get(dogKey, out result);
            Assert.IsTrue(bres);
            Assert.AreEqual(dog, result);

            bres = ao.Get(catKey, out result);
            Assert.IsTrue(bres);
            Assert.AreEqual(cat, result);

            return Task.CompletedTask;
            }

        [Test]
        [TestCase(TestName = "ActorOptions GetBool()")]
        public Task StateTestsBool()
            {
            var ao = new ActorOptions();
            return InternalStateTestsBool(ao);
            }

        private Task InternalStateTestsBool(IActorOptions ao)
            {
            bool bres;
            bool boolResult;

            ao.AddOrUpdate(boolKeyFalse, false);
            ao.AddOrUpdate(boolKeyTrue, true);

            bres = ao.GetBool(boolKeyFalse, out boolResult);
            Assert.IsTrue(bres);
            Assert.AreEqual(false, boolResult);

            bres = ao.GetBool(boolKeyTrue, out boolResult);
            Assert.IsTrue(bres);
            Assert.AreEqual(true, boolResult);

            return Task.CompletedTask;
            }

        [Test]
        [TestCase(TestName = "ActorOptions GetInt()")]
        public Task StateTestsInt()
            {
            var ao = new ActorOptions();
            return InternalStateTestsInt(ao);
            }

        private Task InternalStateTestsInt(IActorOptions ao)
            {
            bool bres;
            int intResult;

            for (int i = 0; i < 10; i++)
                {
                string skey = i.ToString();
                ao.AddOrUpdate(skey, i);
                bres = ao.GetInt(skey, out intResult);
                Assert.IsTrue(bres);
                Assert.AreEqual(i, intResult);
                }

            return Task.CompletedTask;
            }

        [Test]
        [TestCase(TestName = "ActorOptions GetString()")]
        public Task StateTestsString()
            {
            var ao = new ActorOptions();
            return InternalStateTestsString(ao);
            }

        private Task InternalStateTestsString(IActorOptions ao)
            {
            bool bres;
            string result;
            const string svalue1 = "111111111111111111";
            const string svalue2 = "222222222222222222";

            ao.AddOrUpdate(sKey, svalue1);
            bres = ao.GetString(sKey, out result);
            Assert.IsTrue(bres);
            Assert.AreEqual(svalue1, result);

            ao.AddOrUpdate(sKey, svalue2);
            bres = ao.GetString(sKey, out result);
            Assert.IsTrue(bres);
            Assert.AreEqual(svalue2, result);

            bres = ao.GetString(dogKey, out result);
            Assert.IsTrue(bres);
            Assert.AreEqual(dog, result);

            bres = ao.GetString(catKey, out result);
            Assert.IsTrue(bres);
            Assert.AreEqual(cat, result);

            return Task.CompletedTask;
            }

        [Test]
        [TestCase(TestName = "ActorOptions как свойства объекта")]
        public Task TestActorOptions()
            {
            var actor = base.NewPendingActor;
            Assert.IsNotNull(actor.Options);

            InternalStateTests(actor.Options);
            InternalStateTestsString(actor.Options);
            InternalStateTestsInt(actor.Options);
            InternalStateTestsBool(actor.Options);
            return Task.CompletedTask;
            }
        }
    }