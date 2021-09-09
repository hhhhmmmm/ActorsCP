using System;
using System.Text;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    /// Тип сообщения
    /// </summary>
    public enum ActorActionEventType
        {
        /// <summary>
        /// Просто текст - внутренний
        /// </summary>
        SystemNeutral,

        /// <summary>
        /// Просто текст - пришедший извне
        /// </summary>
        Neutral,

        /// <summary>
        /// Предупреждение - пришедшее извне
        /// </summary>
        Warning,

        /// <summary>
        /// Ошибка - пришедшая извне
        /// </summary>
        Error,

        /// <summary>
        /// Ошибка - исключение
        /// </summary>
        Exception
        }
    }