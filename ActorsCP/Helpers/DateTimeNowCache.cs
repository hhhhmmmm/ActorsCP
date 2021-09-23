using System;
using System.Threading;

namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Вспомогательный класс для возврата нового DateTimes каждые нескольео запросов
    /// </summary>
    public static class DateTimeNowCache
        {
        /// <summary>
        /// Количество повторяющихся запросов
        /// </summary>
        private const int MaxCount = 20;

        /// <summary>
        /// Последнее запомненное время
        /// </summary>
        private static DateTime _recentTime = DateTime.Now;

        /// <summary>
        /// Количество пропущенных вызовов
        /// </summary>
        private static volatile int _skipped;

        /// <summary>
        /// DateTime за последние MaxCount вызовов.
        /// </summary>
        /// <returns>Последнее сохраненное DateTime</returns>
        public static DateTime GetDateTime()
            {
            var sc = Interlocked.Increment(ref _skipped);
            if (sc > MaxCount)
                {
                _recentTime = DateTime.Now;
                _skipped = 0;
                }
            return _recentTime;
            }
        }
    }