using System;
using System.Text;

namespace ActorsCP.Logger
    {
    /// <summary>
    /// Глобальный экземпляр логгера - синглтон
    /// </summary>
    public sealed class GlobalActorLogger : ActorLogger
        {
        #region Приватные мемберы

        /// <summary>
        /// Единственный экзмепляр объекта
        /// </summary>
        private static ActorLogger _instance;

        /// <summary>
        /// Локер
        /// </summary>
        private static readonly object _locker = new object();

        /// <summary>
        /// Фабрика экземпляров логгера
        /// </summary>
        private static Func<IActorLogger> _loggerFactory;

        #endregion Приватные мемберы

        /// <summary>
        /// Получить экземпляр объекта
        /// </summary>
        /// <returns>единственный экземпляр</returns>
        public static IActorLogger GetInstance()
            {
            if (_loggerFactory != null)
                {
                return _loggerFactory();
                }

            lock (_locker)
                {
                if (_instance == null)
                    {
                    _instance = new GlobalActorLogger();
                    }
                return _instance;
                }
            }

        /// <summary>
        /// Установить глобальный экземпляр логгера
        /// </summary>
        /// <param name="instance">Глобальный экземпляр логгера</param>
        public static void SetGlobalLoggerInstance(ActorLogger instance)
            {
            if (_instance != null)
                {
                throw new Exception("Глобальный экземпляр уже задан");
                }
            _instance = instance;
            }

        /// <summary>
        /// Установить фабрику логгеров
        /// </summary>
        /// <param name="loggerFactory"></param>
        public static void SetGlobalLoggerFactory(Func<IActorLogger> loggerFactory)
            {
            _loggerFactory = loggerFactory;
            }
        }
    }