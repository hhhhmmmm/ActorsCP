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
                IsErrorEnabled = true;
                }

            if ((Level & ActorLogLevel.Warn) != 0)
                {
                IsWarnEnabled = true;
                }

            if ((Level & ActorLogLevel.Info) != 0)
                {
                IsInfoEnabled = true;
                }

            if ((Level & ActorLogLevel.Debug) != 0)
                {
                IsDebugEnabled = true;
                }
            }

        #endregion Методы

        #region Свойства

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