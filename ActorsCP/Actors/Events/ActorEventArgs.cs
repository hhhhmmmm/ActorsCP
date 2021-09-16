using System;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    /// Базовый класс сообщений объекта
    /// </summary>
    public class ActorEventArgs : EventArgs
        {
        /// <summary>
        /// Дата события в виде строки
        /// </summary>
        private string _stringEventDate;

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

        /// <summary>
        /// Дата события в виде строки
        /// </summary>
        public string EventDateAsString
            {
            get
                {
                if (_stringEventDate == null)
                    {
                    _stringEventDate = string.Intern(EventDate.ToString("HH\\:mm\\:ss\\.ff"));
                    }
                return _stringEventDate;
                }
            }

        #endregion Свойства
        }
    }