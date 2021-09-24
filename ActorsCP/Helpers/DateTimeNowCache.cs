using System;

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

            if (diffMs >= MinDiffMs)
                {
                _recentUtcTime = curUtcTime;
                _recentLocalTime = _recentUtcTime.ToLocalTime();
                }

            return _recentLocalTime;
            }
        }
    }