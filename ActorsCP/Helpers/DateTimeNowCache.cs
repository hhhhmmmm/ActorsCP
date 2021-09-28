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
        /// Последнее запомненное время
        /// </summary>
        private static DateTime _recentLocalTime = DateTime.Now;

        /// <summary>
        /// Шаг квантования, ms
        /// </summary>
        private const double MinDiffMs = 40; // 20 кадров в секунду

        /// <summary>
        /// Шаг квантования, итераций
        /// </summary>
        private const double MaxSpins = 40;

        /// <summary>
        /// Счетчик вызовов
        /// </summary>
        private static volatile int _counter = 0;

        /// <summary>
        /// Последнее запомненное время
        /// </summary>
        private static DateTime _recentUtcTime = DateTime.UtcNow;

        /// <summary>
        /// DateTime за последние MaxCount вызовов.
        /// </summary>
        /// <returns>Последнее сохраненное DateTime</returns>
        public static DateTime GetDateTime()
            {
            DateTime curUtcTime = DateTime.UtcNow;
            var diff = curUtcTime.Subtract(_recentUtcTime);
            var diffMs = diff.TotalMilliseconds; // разница в ms

            var _newCounter = Interlocked.Increment(ref _counter);

            bool conditionReached = false;

            if (diffMs >= MinDiffMs)
                {
                conditionReached = true;
                }
            else
                {
                if (_newCounter >= MaxSpins)
                    {
                    conditionReached = true;
                    }
                }

            if (conditionReached)
                {
                _recentUtcTime = curUtcTime;
                _recentLocalTime = _recentUtcTime.ToLocalTime();
                _newCounter = 0;
                }

            return _recentLocalTime;
            }
        }
    }