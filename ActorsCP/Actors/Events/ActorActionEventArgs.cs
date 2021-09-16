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
            this.MessageText = TrimStartWhitespaces(action);
            this.ActionEventType = actionEventType;
            }

        #endregion Конструкторы

        /// <summary>
        /// Обрезать передние пробелы
        /// </summary>
        /// <param name="str">строка</param>
        /// <returns></returns>
        private static string TrimStartWhitespaces(string str)
            {
            var startIndex = 0;

            //get the starting point of the string without the whitespace
            while (char.IsWhiteSpace(str[startIndex]))
                {
                startIndex += 1;
                }

            if (startIndex == 0)
                {
                return str;
                }

            var endIndex = str.Length - 1;

            endIndex += 1;

            //remove the whitespace
            var stringTrimmed = str.Substring(startIndex, (endIndex - startIndex));
            return stringTrimmed;
            }

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