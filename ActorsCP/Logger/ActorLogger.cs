using System;
using System.Text;

namespace ActorsCP.Logger
    {
    /// <summary>
    ///
    /// </summary>
    public class ActorLogger : IActorLogger
        {
        /// <summary>
        /// Уровень подробности логгера
        /// </summary>
        private LogLevel _logLevel;

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorLogger() : this(LogLevel.Off)
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="logLevel">Уровень подробности логгера</param>
        public ActorLogger(LogLevel logLevel)
            {
            SetLogLevel(logLevel);
            }

        #endregion Конструкторы

        #region Методы

        /// <summary>
        /// Установить уровень подробности логгера
        /// </summary>
        /// <param name="logLevel">Уровень подробности логгера</param>
        public void SetLogLevel(LogLevel logLevel)
            {
            _logLevel = logLevel;
            if ((Level & LogLevel.Fatal) != 0)
                {
                IsFatalEnabled = true;
                }

            if ((Level & LogLevel.Error) != 0)
                {
                IsErrorEnabled = true;
                }

            if ((Level & LogLevel.Warn) != 0)
                {
                IsWarnEnabled = true;
                }

            if ((Level & LogLevel.Info) != 0)
                {
                IsInfoEnabled = true;
                }

            if ((Level & LogLevel.Debug) != 0)
                {
                IsDebugEnabled = true;
                }
            }

        #endregion Методы

        #region Свойства

        /// <summary>
        /// Уровень подробности логгера
        /// </summary>
        public LogLevel Level
            {
            get
                {
                return _logLevel;
                }
            }

        /// <summary>
        /// Логгер включен
        /// </summary>
        public bool IsEnabled
            {
            get
                {
                return Level != LogLevel.Off;
                }
            }

        /// <summary>
        /// Фатальные ошибки включены
        /// </summary>
        public bool IsFatalEnabled
            {
            get;
            private set;
            }

        /// <summary>
        /// Ошибки включены
        /// </summary>
        public bool IsErrorEnabled
            {
            get;
            private set;
            }

        /// <summary>
        /// Предупреждения включены
        /// </summary>
        public bool IsWarnEnabled
            {
            get;
            private set;
            }

        /// <summary>
        /// Информация включена
        /// </summary>
        public bool IsInfoEnabled
            {
            get;
            private set;
            }

        /// <summary>
        /// Уровень отладки включен
        /// </summary>
        public bool IsDebugEnabled
            {
            get;
            private set;
            }

        #endregion Свойства

        #region Методы логгера

        /// <summary>
        /// Фатальная ошибка
        /// </summary>
        /// <param name="fatalText">Текст фатальной ошибки</param>
        public virtual void LogFatal(string fatalText)
            {
            }

        /// <summary>
        /// Исключение
        /// </summary>
        /// <param name="exception">Исключение</param>
        public virtual void LogException(Exception exception)
            {
            LogError(exception?.ToString());
            }

        /// <summary>
        /// Ошибка
        /// </summary>
        /// <param name="errorText">Текст ошибки</param>
        public virtual void LogError(string errorText)
            {
            }

        /// <summary>
        /// Предупреждение
        /// </summary>
        /// <param name="warnText">Текст предупреждения</param>
        public virtual void LogWarn(string warnText)
            {
            }

        /// <summary>
        /// Информация
        /// </summary>
        /// <param name="infoText">Текст информации</param>
        public virtual void LogInfo(string infoText)
            {
            }

        /// <summary>
        /// Отладка
        /// </summary>
        /// <param name="debugText">Текст отладки</param>
        public virtual void LogDebug(string debugText)
            {
            }

        #endregion Методы логгера
        }
    }