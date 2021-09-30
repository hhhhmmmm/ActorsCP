using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using ActorsCP.Logger;

using NLog;

namespace ActorsCPFormsRunner
    {
    /// <summary>
    /// Реализация логгера
    /// </summary>
    public partial class ActorLoggerImplementation : ActorLogger
        {
        #region Приватные мемберы

        /// <summary>
        /// Экземпляр класса логгера
        /// </summary>
        private Logger _logger;

        #endregion Приватные мемберы

        #region Синглтон

        /// <summary>
        /// Единственный экзмепляр объекта
        /// </summary>
        protected static ActorLoggerImplementation _instance;

        /// <summary>
        /// Локер
        /// </summary>
        private static readonly object _locker = new object();

        /// <summary>
        /// Получить экземпляр объекта
        /// </summary>
        /// <returns>единственный экземпляр</returns>
        public static ActorLoggerImplementation GetInstance()
            {
            lock (_locker)
                {
                if (_instance == null)
                    {
                    _instance = new ActorLoggerImplementation();
                    }
                return _instance;
                }
            }

        #endregion Синглтон

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorLoggerImplementation()
            {
            }

        #endregion Конструкторы

        #region Перегружаемые методы

        /// <summary>
        /// Фатальная ошибка
        /// </summary>
        /// <param name="fatalText">Текст фатальной ошибки</param>
        protected override void InternalLogFatal(string fatalText)
            {
            if (_logger != null)
                {
                Task.Run(() =>
                {
                    _logger.Fatal(fatalText);
                }
                );
                }
            else
                {
                var gobalLogger = GetInstance();
                gobalLogger.LogFatal(fatalText);
                }
            }

        /// <summary>
        /// Исключение
        /// </summary>
        /// <param name="exception">Исключение</param>
        public override void InternalLogException(Exception exception)
            {
            if (_logger != null)
                {
                Task.Run(() =>
                {
                    _logger.Error(exception);
                }
                );
                }
            else
                {
                var gobalLogger = GetInstance();
                gobalLogger.LogException(exception);
                }
            }

        /// <summary>
        /// Ошибка
        /// </summary>
        /// <param name="errorText">Текст ошибки</param>
        public override void InternalLogError(string errorText)
            {
            if (_logger != null)
                {
                Task.Run(() =>
                {
                    _logger.Error(errorText);
                }
                );
                }
            else
                {
                var gobalLogger = GetInstance();
                gobalLogger.LogError(errorText);
                }
            }

        /// <summary>
        /// Предупреждение
        /// </summary>
        /// <param name="warnText">Текст предупреждения</param>
        public override void InternalLogWarn(string warnText)
            {
            if (_logger != null)
                {
                Task.Run(() =>
                {
                    _logger.Warn(warnText);
                }
                );
                }
            else
                {
                var gobalLogger = GetInstance();
                gobalLogger.LogWarn(warnText);
                }
            }

        /// <summary>
        /// Информация
        /// </summary>
        /// <param name="infoText">Текст информации</param>
        public override void InternalLogInfo(string infoText)
            {
            if (_logger != null)
                {
                Task.Run(() =>
                {
                    _logger.Info(infoText);
                }
                );
                }
            else
                {
                var gobalLogger = GetInstance();
                gobalLogger.LogInfo(infoText);
                }
            }

        /// <summary>
        /// Отладка
        /// </summary>
        /// <param name="debugText">Текст отладки</param>
        public override void InternalLogDebug(string debugText)
            {
            if (_logger != null)
                {
                Task.Run(() =>
                {
                    _logger.Debug(debugText);
                }
                );
                }
            else
                {
                var gobalLogger = GetInstance();
                gobalLogger.LogDebug(debugText);
                }
            }

        #endregion Перегружаемые методы
        }
    }