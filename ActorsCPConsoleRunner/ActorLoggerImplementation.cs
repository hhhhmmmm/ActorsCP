using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActorsCP.Logger;
using NLog;
using NLog.Fluent;

namespace ActorsCPConsoleRunner
    {
    /// <summary>
    ///
    /// </summary>
    public class ActorLoggerImplementation : ActorLogger
        {
        private Logger _logger;

        /// <summary>
        /// Reconfigures the NLog logging level.
        /// </summary>
        /// <param name="level">The <see cref="ActorLogLevel" /> to be set.</param>
        private static void SetNlogLogLevel(LogLevel level)
            {
            // Uncomment these to enable NLog logging. NLog exceptions are swallowed by default.
            ////NLog.Common.InternalLogger.LogFile = @"C:\Temp\nlog.debug.log";
            ////NLog.Common.InternalLogger.ActorLogLevel = ActorLogLevel.Debug;

            if (level == LogLevel.Off)
                {
                LogManager.DisableLogging();
                }
            else
                {
                if (!LogManager.IsLoggingEnabled())
                    {
                    LogManager.EnableLogging();
                    }

                foreach (var rule in LogManager.Configuration.LoggingRules)
                    {
                    // Iterate over all levels up to and including the target, (re)enabling them.
                    for (int i = level.Ordinal; i <= 5; i++)
                        {
                        rule.EnableLoggingForLevel(LogLevel.FromOrdinal(i));
                        }
                    }
                }

            LogManager.ReconfigExistingLoggers();
            }

        public ActorLoggerImplementation()
            {
            var i = NLogWrapper.GetInstance();
            _logger = i.GetLogger();

            SetLogLevel(ActorLogLevel.Info);

            #region Настройка

            if (!IsEnabled)
                {
                SetNlogLogLevel(LogLevel.Off);
                return;
                }

            if (IsDebugEnabled)
                {
                SetNlogLogLevel(LogLevel.Debug);
                }
            if (IsInfoEnabled)
                {
                SetNlogLogLevel(LogLevel.Info);
                }
            if (IsWarnEnabled)
                {
                SetNlogLogLevel(LogLevel.Warn);
                }
            if (IsErrorEnabled)
                {
                SetNlogLogLevel(LogLevel.Error);
                }
            if (IsFatalEnabled)
                {
                SetNlogLogLevel(LogLevel.Fatal);
                }

            #endregion Настройка
            }

        #region Методы логгера

        /// <summary>
        /// Фатальная ошибка
        /// </summary>
        /// <param name="fatalText">Текст фатальной ошибки</param>
        public override void LogFatal(string fatalText)
            {
            _logger.Fatal(fatalText);
            }

        /// <summary>
        /// Исключение
        /// </summary>
        /// <param name="exception">Исключение</param>
        public override void LogException(Exception exception)
            {
            _logger.Error(exception);
            }

        /// <summary>
        /// Ошибка
        /// </summary>
        /// <param name="errorText">Текст ошибки</param>
        public override void LogError(string errorText)
            {
            _logger.Error(errorText);
            }

        /// <summary>
        /// Предупреждение
        /// </summary>
        /// <param name="warnText">Текст предупреждения</param>
        public override void LogWarn(string warnText)
            {
            _logger.Warn(warnText);
            }

        /// <summary>
        /// Информация
        /// </summary>
        /// <param name="infoText">Текст информации</param>
        public override void LogInfo(string infoText)
            {
            _logger.Info(infoText);
            }

        /// <summary>
        /// Отладка
        /// </summary>
        /// <param name="debugText">Текст отладки</param>
        public override void LogDebug(string debugText)
            {
            _logger.Debug(debugText);
            }

        #endregion Методы логгера
        }
    }