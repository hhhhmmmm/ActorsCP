using System;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    /// Базовый класс сообщений объекта
    /// </summary>
    public class ActorEventArgs : EventArgs
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorEventArgs()
            {
            EventDate = DateTime.Now;
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime EventDate
            {
            get;
            private set;
            }

        #endregion Свойства
        }
    }