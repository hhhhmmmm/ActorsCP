using System;
using System.Diagnostics;
using System.Threading;

using ActorsCP.Helpers;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    /// Базовый класс сообщений объекта
    /// </summary>
    [DebuggerDisplay("AEA = {AEA}")]
    public class ActorEventArgs : EventArgs
        {
        #region Глобальные внутренние объекты

        /// <summary>
        /// Генератор последовательных номеров объектов
        /// </summary>
        private static int s_AEA_global = 0;

        #endregion Глобальные внутренние объекты

        #region Приватные мемберы

        /// <summary>
        /// Уникальный последовательный номер объекта
        /// </summary>
        private readonly int _AEA = 0;

        /// <summary>
        /// Дата события в виде строки
        /// </summary>
        private string _stringEventDate;

        #endregion Приватные мемберы

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorEventArgs()
            {
            _AEA = Interlocked.Increment(ref s_AEA_global); // последовательный номер объекта
            EventDate = DateTimeNowCache.GetDateTime();
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Уникальный последовательный номер объекта
        /// </summary>
        public int AEA
            {
            get
                {
                return _AEA;
                }
            }

        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime EventDate
            {
            get;
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
                    _stringEventDate = FormatEventDate(EventDate);
                    }
                return _stringEventDate;
                }
            }

        #endregion Свойства

        /// <summary>
        /// Отформатировать время
        /// </summary>
        /// <param name="eventDate"></param>
        public static string FormatEventDate(DateTime eventDate)
            {
            var _stringEventDate = string.Intern(eventDate.ToString("HH\\:mm\\:ss\\.ff"));
            return _stringEventDate;
            }
        }
    }