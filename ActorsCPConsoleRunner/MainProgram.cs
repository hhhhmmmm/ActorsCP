using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ActorsCP.Logger;
using ActorsCP.ViewPorts.ConsoleViewPort;

using ActorsCPConsoleRunner.Handlers;

using CommandLine;

namespace ActorsCPConsoleRunner
    {
    internal class MainProgram
        {
        #region Мемберы

        /// <summary>
        /// Результат вызова обработчика handler.Run();
        /// </summary>
        private static int HandlerResult = 0;

        /// <summary>
        /// Уровень логгирования
        /// </summary>
        private static ActorLogLevel s_ActorLogLevel;

        #endregion Мемберы

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
        /// Основная программа
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
            {
            try
                {
                #region Настройка логгера

#if DEBUG
                s_ActorLogLevel = ActorLogLevel.Debug;
#else
            s_ActorLogLevel = ActorLogLevel.Info;
#endif

                var logger = ActorLoggerImplementation.GetInstance();
                bool bres = logger.InitLog("ConsoleRunner");
                logger.SetLogLevel(s_ActorLogLevel);
                ActorLoggerImplementation.ConfigureNLogGlobally(logger);

                GlobalActorLogger.SetGlobalLoggerInstance(logger);
                // GlobalActorLogger.SetGlobalLoggerFactory(CreateLoggerInstance);

                logger.LogInfo("Настройка логгера завершена");

                #endregion Настройка логгера

                var result = Parser.Default.ParseArguments<QueueHandler, CrowdHandler, DebugHandler, ViewPortHandler>(args);

                if (result.Tag == ParserResultType.NotParsed)
                    {
                    result.WithNotParsed<object>(ErrorHandler);
                    Environment.Exit(-1);
                    }

                result.WithParsed<QueueHandler>((x) => { ConfigureAndRun(x); });
                result.WithParsed<CrowdHandler>((x) => { ConfigureAndRun(x); });
                result.WithParsed<DebugHandler>((x) => { ConfigureAndRun(x); });
                result.WithParsed<ViewPortHandler>((x) => { ConfigureAndRun(x); });
                } // end try
            catch (Exception ex)
                {
                ConsoleActorViewPort.ReportException(ex);
                }
            finally
                {
                ConsoleActorViewPort.RestoreColors();

                // SetTitle("завершен");
                Environment.Exit(HandlerResult);
                }
            }

        #region Обработчики глаголов

        /// <summary>
        /// Настроить и запустить
        /// </summary>
        /// <param name="options"></param>
        private static void ConfigureAndRun(HandlerBase options)
            {
            Console.ForegroundColor = (ConsoleColor)ConsoleActorViewPort.MessageColor;

            HandlerResult = options.Run();
            }

        /// <summary>
        /// Обработчик ошибок
        /// </summary>
        /// <param name="errors"></param>
        private static void ErrorHandler(IEnumerable<Error> errors)
            {
            bool stopProcessing = errors.Any(e => e.StopsProcessing);

            if (errors.Any(e => e.Tag == ErrorType.HelpRequestedError || e.Tag == ErrorType.VersionRequestedError))
                {
                return;
                }

            foreach (var error in errors)
                {
                var message = ConsoleActorViewPort.CreateErrorMessage(error.ToString());
                ConsoleActorViewPort.WriteLineToConsole(message);
                }
            }

        #endregion Обработчики глаголов
        }
    }