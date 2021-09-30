using System;
using System.Collections.Generic;
using System.Text;

namespace ActorsCP.Options
    {
    /// <summary>
    /// Словарь отладочных слов понимаемых объектами
    /// </summary>
    public static class ActorDebugKeywords
        {
        /// <summary>
        /// Отладка сообщений вьюпорта об изменении состояния набора
        /// </summary>
        public const string ViewPort_DebugStateChangedEvent = "ViewPort_DebugStateChangedEvent";

        /// <summary>
        /// Отладка очереди сообщений
        /// </summary>
        public const string QueueBufferT_Debug = "QueueBufferT_Debug";
        }
    }