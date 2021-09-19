using System;
using System.Text;

namespace ActorsCP.Logger
    {
    /// <summary>
    /// Уровень подробности логгера
    /// </summary>
    [Flags]
    public enum ActorLogLevel
        {
        /// <summary>
        /// Отключен
        /// </summary>
        Off = 0,

        /// <summary>
        /// Фатальные ошибки
        /// </summary>
        Fatal = 1,

        /// <summary>
        /// Ошибки и исключения
        /// </summary>
        Error = 2,

        /// <summary>
        /// Предупреждения
        /// </summary>
        Warn = 4,

        /// <summary>
        /// Информация
        /// </summary>
        Info = 8,

        /// <summary>
        /// Уровень отладки
        /// </summary>
        Debug = 16
        };
    }