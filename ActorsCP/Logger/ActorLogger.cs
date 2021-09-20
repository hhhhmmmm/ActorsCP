using System;
using System.Text;
using ActorsCP.Helpers;

namespace ActorsCP.Logger
    {
    /// <summary>
    /// Базовый класс логгера
    /// </summary>
    public class ActorLogger : DisposableImplementation<ActorLogger>, IActorLogger
        {
        /// <summary>
        /// Уровень подробности логгера
        /// </summary>
        private readonly ActorLoggerOptions _actorLoggerOptions = new ActorLoggerOptions();

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorLogger() : this(ActorLogLevel.Off)
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="actorLogLevel">Уровень подробности логгера</param>
        public ActorLogger(ActorLogLevel actorLogLevel)
            {
            SetLogLevel(actorLogLevel);
            }

        #endregion Конструкторы

        #region Методы

        /// <summary>
        /// Установить уровень подробности логгера
        /// </summary>
        /// <param name="actorLogLevel">Уровень подробности логгера</param>
        public void SetLogLevel(ActorLogLevel actorLogLevel)
            {
            _actorLoggerOptions.SetLogLevel(actorLogLevel);
            }

        #endregion Методы

        #region Свойства

        /// <summary>
        /// Уровень подробности логгера
        /// </summary>
        public ActorLoggerOptions LoggerOptions
            {
            get
                {
                return _actorLoggerOptions;
                }
            }

        #endregion Свойства

        #region Перегружаемые методы

        /// <summary>
        /// Фатальная ошибка
        /// </summary>
        /// <param name="fatalText">Текст фатальной ошибки</param>
        protected virtual void InternalLogFatal(string fatalText)
            {
            }

        /// <summary>
        /// Исключение
        /// </summary>
        /// <param name="exception">Исключение</param>
        public virtual void InternalLogException(Exception exception)
            {
            }

        /// <summary>
        /// Ошибка
        /// </summary>
        /// <param name="errorText">Текст ошибки</param>
        public virtual void InternalLogError(string errorText)
            {
            }

        /// <summary>
        /// Предупреждение
        /// </summary>
        /// <param name="warnText">Текст предупреждения</param>
        public virtual void InternalLogWarn(string warnText)
            {
            }

        /// <summary>
        /// Информация
        /// </summary>
        /// <param name="infoText">Текст информации</param>
        public virtual void InternalLogInfo(string infoText)
            {
            }

        /// <summary>
        /// Отладка
        /// </summary>
        /// <param name="debugText">Текст отладки</param>
        public virtual void InternalLogDebug(string debugText)
            {
            }

        #endregion Перегружаемые методы

        #region Методы логгера

        /// <summary>
        /// Фатальная ошибка
        /// </summary>
        /// <param name="fatalText">Текст фатальной ошибки</param>
        public void LogFatal(string fatalText)
            {
            if (LoggerOptions.IsFatalEnabled)
                {
                InternalLogFatal(fatalText);
                }
            }

        /// <summary>
        /// Исключение
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void LogException(Exception exception)
            {
            if (LoggerOptions.IsErrorEnabled)
                {
                InternalLogException(exception);
                }
            }

        /// <summary>
        /// Ошибка
        /// </summary>
        /// <param name="errorText">Текст ошибки</param>
        public void LogError(string errorText)
            {
            if (LoggerOptions.IsErrorEnabled)
                {
                try
                    {
                    InternalLogError(errorText);
                    }
                catch (Exception exception)
                    {
                    LogException(exception);
                    }
                }
            }

        /// <summary>
        /// Предупреждение
        /// </summary>
        /// <param name="warnText">Текст предупреждения</param>
        public void LogWarn(string warnText)
            {
            if (LoggerOptions.IsWarnEnabled)
                {
                try
                    {
                    InternalLogWarn(warnText);
                    }
                catch (Exception exception)
                    {
                    LogException(exception);
                    }
                }
            }

        /// <summary>
        /// Информация
        /// </summary>
        /// <param name="infoText">Текст информации</param>
        public void LogInfo(string infoText)
            {
            if (LoggerOptions.IsInfoEnabled)
                {
                try
                    {
                    InternalLogInfo(infoText);
                    }
                catch (Exception exception)
                    {
                    LogException(exception);
                    }
                }
            }

        /// <summary>
        /// Отладка
        /// </summary>
        /// <param name="debugText">Текст отладки</param>
        public void LogDebug(string debugText)
            {
            if (LoggerOptions.IsDebugEnabled)
                {
                try
                    {
                    InternalLogDebug(debugText);
                    }
                catch (Exception exception)
                    {
                    LogException(exception);
                    }
                }
            }

        #endregion Методы логгера
        }
    }