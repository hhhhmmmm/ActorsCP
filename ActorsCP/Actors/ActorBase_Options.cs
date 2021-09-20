using System;
using System.Collections.Generic;
using System.Text;
using ActorsCP.Options;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Частичная реализация ActorBase
    /// </summary>
    public partial class ActorBase
        {
        #region Опции объекта

        private IActorOptions _actorOptions = null;

        /// <summary>
        /// Опции объекта
        /// </summary>
        public IActorOptions Options
            {
            get
                {
                lock (Locker)
                    {
                    if (_actorOptions != null)
                        {
                        return _actorOptions;
                        }

                    var gactorOptions = GlobalActorOptions.GetInstance();
                    return gactorOptions;
                    }
                }
            }

        #endregion Опции объекта

        #region Опции отладки объекта

        private IActorOptions _actorDebugOptions = null;

        /// <summary>
        /// Опции отладки объекта
        /// </summary>
        public IActorOptions DebugOptions
            {
            get
                {
                lock (Locker)
                    {
                    if (_actorDebugOptions != null)
                        {
                        return _actorDebugOptions;
                        }

                    var gactorDebugOptions = GlobalActorDebugOptions.GetInstance();
                    return gactorDebugOptions;
                    }
                }
            }

        #endregion Опции отладки объекта

        /// <summary>
        /// Установить персональные опции объекта
        /// </summary>
        /// <param name="actorOptions">Опции объекта</param>
        public void SetActorOptions(IActorOptions actorOptions)
            {
            _actorOptions = actorOptions;
            }

        /// <summary>
        /// Установить персональные опции отладки объекта
        /// </summary>
        /// <param name="actorDebugOptions">Опции отладки объекта</param>
        public void SetActorDebugOptions(IActorOptions actorDebugOptions)
            {
            _actorDebugOptions = actorDebugOptions;
            }
        }
    }