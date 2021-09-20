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
        /// <summary>
        /// Экземпляр логгера
        /// </summary>
        private IActorLogger _actorLogger;

        private ActorLoggerOptions _actorLoggerOptions;

        #region Свойства

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
            }

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
            }

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