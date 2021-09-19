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

        #region Свойства

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
        }
    }