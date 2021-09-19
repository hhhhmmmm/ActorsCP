using System;
using System.Text;

namespace ActorsCP.Logger
    {
    /// <summary>
    /// Опции логгера
    /// </summary>
    public sealed class ActorLoggerOptions
        {
        /// <summary>
        /// Уровень подробности логгера
        /// </summary>
        private ActorLogLevel _actorLogLevel;

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorLoggerOptions() : this(ActorLogLevel.Off)
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="actorLogLevel">Уровень подробности логгера</param>
        public ActorLoggerOptions(ActorLogLevel actorLogLevel)
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
        }
    }