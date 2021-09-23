using System;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    /// Вспомогательный класс для передачи сообщений от объекта
    /// </summary>
    public class ActorActionEventArgs : ActorEventArgs
        {
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public ActorActionEventType ActionEventType
            {
            get;
            private set;
            }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string MessageText
            {
            get;
            protected set;
            }

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        public ActorActionEventArgs(string messageText)
            : this(messageText, ActorActionEventType.Neutral)
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="actionEventType">Тип сообщения</param>
        public ActorActionEventArgs(ActorActionEventType actionEventType)
            {
            this.ActionEventType = actionEventType;
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        /// <param name="actionEventType">Тип сообщения</param>
        public ActorActionEventArgs(string action, ActorActionEventType actionEventType)
            {
            MessageText = action;
            ActionEventType = actionEventType;
            }

        #endregion Конструкторы

        #region Перегружаемые методы

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            {
            return $"{ActionEventType} '{MessageText}' ";
            }

        #endregion Перегружаемые методы
        }
    }