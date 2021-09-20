using System;
using System.Collections.Generic;
using System.Text;

namespace ActorsCP.Logger
    {
    /// <summary>
    /// Преднастроенные опции логгера
    /// </summary>
    public static class DefaultErrorLoggerOptions
        {
        /// <summary>
        /// Логгер уровня Ошибки и выше
        /// </summary>
        public static ActorLoggerOptions ErrorLoggerOptions
            {
            get;
            } = new ActorLoggerOptions(ActorLogLevel.Error);
        } // end class DefaultErrorLoggerOptions
    } // end namespace ActorsCP.Logger