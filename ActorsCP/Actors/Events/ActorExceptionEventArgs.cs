using System;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    /// Вспомогательный класс для передачи исключений через ивенты
    /// </summary>
    public sealed class ActorExceptionEventArgs : ActorActionEventArgs
        {
        /// <summary>
        /// Исключение
        /// </summary>
        public Exception Exception
            {
            get;
            private set;
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="exception">Исключение</param>
        public ActorExceptionEventArgs(Exception exception) : base(ActorActionEventType.Exception)
            {
            this.Exception = exception;
            base.Action = exception.ToString();
            }
        }
    }