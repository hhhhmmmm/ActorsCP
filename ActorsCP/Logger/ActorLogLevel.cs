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
        Error = 2 + 1,

        /// <summary>
        /// Предупреждения
        /// </summary>
        Warn = 4 + 2 + 1,

        /// <summary>
        /// Информация
        /// </summary>
        Info = 8 + 4 + 2 + 1,

        /// <summary>
        /// Уровень отладки
        /// </summary>
        Debug = 16 + 8 + 4 + 2 + 1,
        };
    }