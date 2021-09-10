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
        /// Просто текст - для отладчика и лога
        /// </summary>
        Debug,

        /// <summary>
        /// Просто текст
        /// </summary>
        Neutral,

        /// <summary>
        /// Предупреждение
        /// </summary>
        Warning,

        /// <summary>
        /// Ошибка
        /// </summary>
        Error,

        /// <summary>
        /// Ошибка - исключение
        /// </summary>
        Exception
        }
    }