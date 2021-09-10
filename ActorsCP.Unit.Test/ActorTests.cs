using ActorsCP.TestActors;
using ActorsCP.Actors;

using NUnit.Framework;
using System.Threading.Tasks;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    // [Description("Тесты актора")]
    [Category("Тесты актора")]
    public class ActorTests : TestBase
        {
        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void Init()
            {
            }

        [Test]
        [TestCase(TestName = "10. Тесты состояния актора пустого актора")]
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
            var exStart = new ExceptionActor();
            exStart.ExceptionOnStart = true;
            bres = await exStart.StartAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exStart.State, ActorState.Terminated);

            // стоп
            var exStop = new ExceptionActor();
            exStop.ExceptionOnStop = true;
            bres = await exStop.StartAsync();
            Assert.IsTrue(bres);
            Assert.AreEqual(exStop.State, ActorState.Started);

            bres = await exStop.StopAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exStop.State, ActorState.Terminated);

            // run
            var exRun = new ExceptionActor();
            exRun.ExceptionOnRun = true;
            bres = await exRun.RunAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exRun.State, ActorState.Terminated);

            // Cleanup
            var exCleanup = new ExceptionActor();
            exCleanup.ExceptionOnRunCleanupBeforeTerminationAsync = true;
            bres = await exCleanup.RunAsync();
            Assert.IsFalse(bres);
            Assert.AreEqual(exCleanup.State, ActorState.Terminated);
            }
        }
    }

//[Test]
///// <param name="useProxy">Использовать прокси</param>
///// <param name="scheme"></param>
///// <param name="serverIPAddress">IP адрес прокси-сервера</param>
///// <param name="serverPort">Порт прокси-сервера</param>
///// <param name="useCustomNetworkCredential">Использовать имя пользователя и пароль </param>
///// <param name="networkCredentialUserName">Имя пользователя прокси</param>
///// <param name="networkCredentialPassword">Пароль пользователя прокси</param>
///// <param name="networkCredentialDomain">Домен пользователя прокси</param>
///// <param name="bypassProxyOnLocal">Не использовать прокси для локальных адресов</param>
///// <param name="bypassList">Список адресов для которых прокси не используется</param>
///// <param name="wantedStatusCode">Ожидаемый код ответа</param>
//[TestCase(true, "192.168.200.1", 3128, true, "tecmint", "12345", "", false, null, HttpStatusCode.OK, TestName = "ProxyTest standalone - Вызов GET 192.168.200.1")]
//[TestCase(true, "192.168.200.1", 3128, true, "tecmint", "123456", "", false, null, HttpStatusCode.ProxyAuthenticationRequired, TestName = "ProxyTest standalone - Вызов GET 192.168.200.1 неверный пароль")]
//[TestCase(true, "192.0.2.13", 3128, true, "tecmint", "12345", "", false, null, HttpStatusCode.OK, TestName = "ProxyTest standalone - Вызов GET 192.0.2.13")]
//[TestCase(false, "192.0.2.13", 3128, true, "tecmint", "12345", "", false, null, HttpStatusCode.OK, TestName = "ProxyTest standalone - Вызов GET 192.0.2.13 без прокси")]
//[TestCase(true, "173.249.35.163", 10010, false, "", "", "", false, null, HttpStatusCode.OK, TestName = "ProxyTest standalone - 173.249.35.163 10010")]
//[TestCase(true, "51.158.98.121", 8811, false, "", "", "", false, null, HttpStatusCode.OK, TestName = "ProxyTest standalone - 51.158.98.121")]
//[TestCase(true, "173.249.35.163", 1448, false, "", "", "", false, null, HttpStatusCode.OK, TestName = "ProxyTest standalone - 173.249.35.163 1448")]
//[TestCase(true, "182.52.31.58", 8080, false, "", "", "", false, null, HttpStatusCode.OK, TestName = "ProxyTest standalone - 182.52.31.58")]
//[TestCase(true, "157.230.45.141", 8080, false, "", "", "", false, null, HttpStatusCode.OK, TestName = "ProxyTest standalone - 157.230.45.141")]
//[TestCase(true, "45.248.94.60", 40886, false, "elite", "proxy", "", false, null, HttpStatusCode.OK, TestName = "ProxyTest standalone - 45.248.94.60")]
//[Category("ProxyTest standalone")]
//public async Task GetUsingProxyGoogleStandalone
//    (
//    bool useProxy,
//    string serverIPAddress,
//    int serverPort,
//    bool useCustomNetworkCredential,
//    string networkCredentialUserName,
//    string networkCredentialPassword,
//    string networkCredentialDomain,
//    bool bypassProxyOnLocal,
//    string[] bypassList,
//    int wantedStatusCode
//    )
//    {
//        //Assert.AreEqual(wantedStatusCode, (int)consumer.StatusCode);
//        //Debug.WriteLine(consumer.StringResult);

//    }