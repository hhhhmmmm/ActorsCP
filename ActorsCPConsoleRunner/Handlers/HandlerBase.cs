using System;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.Helpers;
using ActorsCP.ViewPorts.ConsoleViewPort;

using CommandLine;

namespace ActorsCPConsoleRunner.Handlers
    {
    /// <summary>
    /// Базовый класс опций
    /// </summary>
    public class HandlerBase : IMessageChannel
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parameter"></param>
        public HandlerBase()
            {
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Вьюпорт по умолчанию
        /// </summary>
        public ConsoleActorViewPort DefaultViewPort
            {
            get;
            private set;
            }

        /// <summary>
        ///
        /// </summary>
        [Option('n', "nomessages", Required = false, HelpText = "Не выдавать сообщения вьюпорта на экран")]
        public bool NoOutMessagesDefault
            {
            get;
            set;
            }

        #endregion Свойства

        #region Методы

        /// <summary>
        /// Метод запуска
        /// </summary>
        public int Run()
            {
            DefaultViewPort = new ConsoleActorViewPort();
            DefaultViewPort.Init();
            DefaultViewPort.NoOutMessages = NoOutMessagesDefault;

            ActorTime actorTime = default;
            actorTime.SetStartDate();
            var task = InternalRun();
            int result = task.Result;
            actorTime.SetEndDate();

            DefaultViewPort.NoOutMessages = false;

            var time = actorTime.ShortTimeInterval;
            var str = $"Полное время обработчика: {time}";
            DefaultViewPort?.RaiseWarning(str);
            return result;
            }

        #endregion Методы

        /// <summary>
        /// Метод запуска
        /// </summary>
        protected virtual Task<int> InternalRun()
            {
            Console.WriteLine($"InternalRun() - {this.ToString()}");
            return Task.FromResult(0);
            }

        #region Реализация интерфейса IMessageChannel

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="MessageText">Текст сообщения</param>
        public void RaiseMessage(string MessageText)
            {
            DefaultViewPort?.RaiseMessage(MessageText);
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="MessageText">Текст сообщения</param>
        public void RaiseWarning(string MessageText)
            {
            DefaultViewPort?.RaiseWarning(MessageText);
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="ErrorText">Текст сообщения об ошибке</param>
        public void RaiseError(string ErrorText)
            {
            DefaultViewPort?.RaiseError(ErrorText);
            }

        public void RaiseDebug(string debugText)
            {
            DefaultViewPort?.RaiseDebug(debugText);
            }

        public void RaiseException(Exception exception)
            {
            DefaultViewPort?.RaiseException(exception);
            }

        #endregion Реализация интерфейса IMessageChannel

        //
        } // end class OptionsBase
    } // end namespace ActorsCPConsoleRunner.Handlers