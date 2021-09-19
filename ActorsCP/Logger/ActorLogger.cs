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
        private ActorLogLevel _actorLogLevel;

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
            _actorLogLevel = actorLogLevel;
            if ((Level & ActorLogLevel.Fatal) != 0)
                {
                IsFatalEnabled = true;
                }

            if ((Level & ActorLogLevel.Error) != 0)
                {
                IsFatalEnabled = true;
                IsErrorEnabled = true;
                }

            if ((Level & ActorLogLevel.Warn) != 0)
                {
                IsFatalEnabled = true;
                IsErrorEnabled = true;
                IsWarnEnabled = true;
                }

            if ((Level & ActorLogLevel.Info) != 0)
                {
                IsFatalEnabled = true;
                IsErrorEnabled = true;
                IsWarnEnabled = true;
                IsInfoEnabled = true;
                }

            if ((Level & ActorLogLevel.Debug) != 0)
                {
                IsFatalEnabled = true;
                IsErrorEnabled = true;
                IsWarnEnabled = true;
                IsInfoEnabled = true;
                IsDebugEnabled = true;
                }
            }

        #endregion Методы

        #region Свойства

        /// <summary>
        /// Логгер уровня Ошибки и выше
        /// </summary>
        public static IActorLogger ErrorLogger
            {
            get;
            } = new ActorLogger(ActorLogLevel.Error);

        /// <summary>
        /// Уровень подробности логгера
        /// </summary>
        public ActorLogLevel Level
            {
            get
                {
                return _actorLogLevel;
                }
            }

        /// <summary>
        /// Логгер включен
        /// </summary>
        public bool IsEnabled
            {
            get
                {
                return Level != ActorLogLevel.Off;
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
            if (IsFatalEnabled)
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
            if (IsErrorEnabled)
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
            if (IsErrorEnabled)
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
            if (IsWarnEnabled)
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
            if (IsInfoEnabled)
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
            if (IsDebugEnabled)
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