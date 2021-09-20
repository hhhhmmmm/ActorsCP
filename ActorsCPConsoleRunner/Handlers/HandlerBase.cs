using System;
using System.Text;
using System.Threading.Tasks;
using ActorsCP.Actors;
using ActorsCP.Helpers;
using ActorsCP.Logger;
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
        ///Выдавать сообщения вьюпорта на экран
        /// </summary>
        [Option('m', "messages", Required = false, Default = false, HelpText = "Выдавать сообщения вьюпорта на экран")]
        public bool OutMessagesDefault
            {
            get;
            set;
            }

        /// <summary>
        ///Выдавать сообщения вьюпорта на экран
        /// </summary>
        [Option('l', "log", Required = false, Default = false, HelpText = "Писать лог в файл лога")]
        public bool WriteLog
            {
            get;
            set;
            }

        public bool NoOutMessagesDefault
            {
            get
                {
                return !OutMessagesDefault;
                }
            }

        #endregion Свойства

        #region Методы

        /// <summary>
        /// Фабрика логгеров
        /// </summary>
        /// <returns></returns>
        private static IActorLogger CreateLoggerInstance()
            {
            //var i = new ActorLoggerImplementation();
            //i.SetLogLevel(s_ActorLogLevel);
            //return i;
            return null;
            }

        /// <summary>
        /// Метод запуска
        /// </summary>
        public int Run()
            {
            #region Настройка логгера

            //#if DEBUG
            MainProgram.ActorLogLevel = ActorLogLevel.Debug;
            //#else
            //           MainProgram.ActorLogLevel = ActorLogLevel.Info;
            //#endif

            if (WriteLog)
                {
                var logger = ActorLoggerImplementation.GetInstance();
                bool bres = logger.InitLog("ConsoleRunner");
                logger.SetLogLevel(MainProgram.ActorLogLevel);
                ActorLoggerImplementation.ConfigureNLogGlobally(logger);

                GlobalActorLogger.SetGlobalLoggerInstance(logger);
                // GlobalActorLogger.SetGlobalLoggerFactory(CreateLoggerInstance);

                logger.LogFatal("Настройка логгера завершена");
                } // end WriteLog

            #endregion Настройка логгера

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