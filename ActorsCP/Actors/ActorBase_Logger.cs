using System;
using System.Text;
using ActorsCP.Logger;

namespace ActorsCP.Actors
    {
    /// <summary>
    ///
    /// </summary>
    public partial class ActorBase
        {
        #region Свойства

        #region LoggerOptions

        /// <summary>
        /// Уровень подробности логгера
        /// </summary>
        private ActorLoggerOptions _actorLoggerOptions;

        /// <summary>
        /// Уровень подробности логгера
        /// </summary>
        public ActorLoggerOptions LoggerOptions
            {
            get
                {
                if (_actorLoggerOptions != null)
                    {
                    return _actorLoggerOptions;
                    }

                if (_actorLogger != null)
                    {
                    var options = _actorLogger.LoggerOptions;
                    if (options != null)
                        {
                        return options;
                        }
                    }

                var globalActorLogger = GlobalActorLogger.GetInstance();
                return globalActorLogger.LoggerOptions;
                }
            set
                {
                SetLoggerOptions(value);
                }
            }

        #endregion LoggerOptions

        #region Logger

        /// <summary>
        /// Экземпляр логгера
        /// </summary>
        private IActorLogger _actorLogger;

        /// <summary>
        /// Экземпляр логгера
        /// </summary>
        protected IActorLogger Logger
            {
            get
                {
                if (_actorLogger != null)
                    {
                    return _actorLogger;
                    }

                var globalActorLogger = GlobalActorLogger.GetInstance();
                return globalActorLogger;
                }
            set
                {
                SetLogger(value);
                }
            }

        #endregion Logger

        #endregion Свойства

        /// <summary>
        /// Установить персональный логгер для класса
        /// </summary>
        public void SetLogger(IActorLogger actorLogger)
            {
            _actorLogger = actorLogger;
            }

        /// <summary>
        /// Установить уровень подробности логгера
        /// </summary>
        public void SetLoggerOptions(ActorLoggerOptions actorLoggerOptions)
            {
            _actorLoggerOptions = actorLoggerOptions;
            }
        }
    }