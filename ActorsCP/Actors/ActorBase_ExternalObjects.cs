using System;
using System.Collections.Concurrent;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Частичная реализация - внешние объекты
    /// </summary>
    public partial class ActorBase
        {
        /// <summary>
        /// Словарь для хранения слабых ссылок на внешние объекты
        /// </summary>
        private ConcurrentDictionary<string, WeakReference> _externalObjects = null;

        #region Свойства

        /// <summary>
        /// Список для хранения внешних объектов
        /// </summary>
        public ConcurrentDictionary<string, WeakReference> ExternalObjects
            {
            get
                {
                lock (Locker)
                    {
                    if (_externalObjects == null)
                        {
                        _externalObjects = new ConcurrentDictionary<string, WeakReference>();
                        }
                    }
                return _externalObjects;
                }
            }

        #endregion Свойства
        } // end class ActorBase
    } // end namespace ActorsCP.Actors