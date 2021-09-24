using System;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.dotNET.ViewPorts.ConsoleViewPort;
using ActorsCP.Helpers;
using ActorsCP.Logger;
using ActorsCP.Options;
using ActorsCP.ViewPorts;
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
        public ActorViewPortBase DefaultViewPort
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

        /// <summary>
        ///Выдавать сообщения вьюпорта на экран
        /// </summary>
        [Option('v', "viewport", Required = false, Default = "queue", HelpText = "Тип вьюпорта - tpl, cp, queue")]
        public string ViewportType
            {
            get;
            set;
            }

        /// <summary>
        /// Многословность объекта - о каких событиях он будет сообщать
        /// </summary>
        [Option('z', "verbosity", Required = false, HelpText = "Многословность объекта 1-2-4-8")]
        public string ActorVerbosity
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
                logger.InitLog("ConsoleRunner");
                logger.SetLogLevel(MainProgram.ActorLogLevel);
                ActorLoggerImplementation.ConfigureNLogGlobally(logger);

                GlobalActorLogger.SetGlobalLoggerInstance(logger);
                // GlobalActorLogger.SetGlobalLoggerFactory(CreateLoggerInstance);

                logger.LogFatal("Настройка логгера завершена");
                } // end WriteLog

            #endregion Настройка логгера

            #region Создание вьюпорта

            if (ViewportType.Equals("Cp", StringComparison.OrdinalIgnoreCase))
                {
                dynamic vp = new ConsoleActorViewPort();
                DefaultViewPort = vp;
                vp.Init();

                vp.NoOutMessages = false;
                WriteWarningMessage("Вьюпорт типа ConsoleActorViewPort");
                }
            else
            if (ViewportType.Equals("Tpl", StringComparison.OrdinalIgnoreCase))
                {
                var vp = new BufferedConsoleActorViewPort();
                vp.UseQueueForBuffering = false;
                DefaultViewPort = vp;
                vp.Init();

                vp.NoOutMessages = false;
                WriteWarningMessage("Вьюпорт типа BufferedConsoleActorViewPort - tpl");
                }
            else
            if (ViewportType.Equals("queue", StringComparison.OrdinalIgnoreCase))
                {
                var vp = new BufferedConsoleActorViewPort();
                vp.UseQueueForBuffering = true;
                DefaultViewPort = vp;
                vp.Init();

                vp.NoOutMessages = false;
                WriteWarningMessage("Вьюпорт типа BufferedConsoleActorViewPort - queue");
                }
            else
            if (string.IsNullOrEmpty(ViewportType))
                {
                var vp = new BufferedConsoleActorViewPort();
                vp.UseQueueForBuffering = true;
                DefaultViewPort = vp;
                vp.Init();

                vp.NoOutMessages = false;
                WriteWarningMessage("Вьюпорт по умолчанию - типа BufferedConsoleActorViewPort - queue");
                }

            DefaultViewPort.NoOutMessages = NoOutMessagesDefault;

            #endregion Создание вьюпорта

            var go = GlobalActorOptions.GetInstance();

            if (!string.IsNullOrEmpty(ActorVerbosity))
                {
                var iActorVerbosity = int.Parse(ActorVerbosity);
                go.AddOrUpdate(ActorKeywords.ActorVerbosity, iActorVerbosity);
                WriteWarningMessage($"ActorVerbosity: {iActorVerbosity}");
                }
            else
                {
                WriteWarningMessage($"ActorVerbosity: по умолчанию");
                }

            ActorTime actorTime = default;
            actorTime.SetStartDate();
            var task = InternalRun();
            int result = task.Result;
            actorTime.SetEndDate();

            DefaultViewPort.Terminate();
            DefaultViewPort.ValidateStatistics();

            #region Статистика

            var stat = DefaultViewPort.СurrentExecutionStatistics.TextStatistics;
            WriteWarningMessage(stat);

            #endregion Статистика

            DefaultViewPort.Dispose();
            DefaultViewPort = null;

            #region Полное время обработчика

            var time = actorTime.ShortTimeInterval;
            var stime = $"Полное время работы InternalRun(): {time}";
            WriteWarningMessage(stime);

            #endregion Полное время обработчика

            return result;
            }

        #endregion Методы

        /// <summary>
        /// Сообщение
        /// </summary>
        /// <param name="text"></param>
        protected void WriteWarningMessage(string text)
            {
            var mstat = ConsoleViewPortStatics.CreateWarningMessage(text);
            ConsoleViewPortStatics.WriteLineToConsole(mstat);
            }

        /// <summary>
        /// Сообщение
        /// </summary>
        /// <param name="text"></param>
        protected void WriteErrorMessage(string text)
            {
            var mstat = ConsoleViewPortStatics.CreateErrorMessage(text);
            ConsoleViewPortStatics.WriteLineToConsole(mstat);
            }

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
        /// <param name="messageText">Текст сообщения</param>
        public void RaiseMessage(string messageText)
            {
            dynamic d = DefaultViewPort;
            d?.RaiseMessage(messageText);
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        public void RaiseWarning(string messageText)
            {
            dynamic d = DefaultViewPort;
            d?.RaiseWarning(messageText);
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="errorText">Текст сообщения об ошибке</param>
        public void RaiseError(string errorText)
            {
            dynamic d = DefaultViewPort;
            d?.RaiseError(errorText);
            }

        public void RaiseDebug(string debugText)
            {
            dynamic d = DefaultViewPort;
            d?.RaiseDebug(debugText);
            }

        public void RaiseException(Exception exception)
            {
            dynamic d = DefaultViewPort;
            d?.RaiseException(exception);
            }

        #endregion Реализация интерфейса IMessageChannel

        //
        } // end class OptionsBase
    } // end namespace ActorsCPConsoleRunner.Handlers