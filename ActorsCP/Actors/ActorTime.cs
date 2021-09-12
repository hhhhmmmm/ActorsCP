using System;
using System.Runtime.InteropServices;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Структура хранит минимальное время начала выполнения задач и максимальное время окончания
    /// выполнения задач из списка выполнения
    /// </summary>
    [ComVisible(false)]
    public struct ActorTime
        {
        /// <summary>
        /// Есть дата начала выполнения
        /// </summary>
        public bool HasStartDate
            {
            get;
            set;
            }

        /// <summary>
        /// Минимальное время начала выполнения задач
        /// </summary>
        public DateTime StartDate
            {
            get;
            set;
            }

        /// <summary>
        /// Есть дата окончания выполнения
        /// </summary>
        public bool HasEndDate
            {
            get;
            set;
            }

        /// <summary>
        /// Максимальное время окончания выполнения задач
        /// </summary>
        public DateTime EndDate
            {
            get;
            set;
            }

        /// <summary>
        /// Установить текущую дату в качестве стартовой
        /// </summary>
        public void SetStartDate()
            {
            HasStartDate = true;
            StartDate = DateTime.Now;
            }

        /// <summary>
        /// Установить текущую дату в качестве конечной
        /// </summary>
        public void SetEndDate()
            {
            HasEndDate = true;
            EndDate = DateTime.Now;
            }

        /// <summary>
        /// Есть верный диапазон времени
        /// </summary>
        public bool IsValidRange
            {
            get
                {
                return HasStartDate && HasEndDate;
                }
            }

        /// <summary>
        /// Разность дат в миллисекундах
        /// </summary>
        public double TotalMilliseconds
            {
            get
                {
                if (HasStartDate && HasEndDate)
                    {
                    var ExecutionTimeSpan = EndDate - StartDate;
                    var TotalMilliseconds = ExecutionTimeSpan.Duration().TotalMilliseconds;
                    return TotalMilliseconds;
                    }
                return 0;
                }
            }

        /// <summary>
        /// Разность дат в виде строки
        /// </summary>
        public string TimeInterval
            {
            get
                {
                if (HasStartDate && HasEndDate)
                    {
                    var ExecutionTimeSpan = EndDate - StartDate;
                    var Duration = ExecutionTimeSpan.Duration();
                    var DurationTime = Duration.ToString();
                    return DurationTime;
                    }
                return string.Empty;
                }
            }

        /// <summary>
        /// Разность дат в виде строки
        /// </summary>
        public string ShortTimeInterval
            {
            get
                {
                if (HasStartDate && HasEndDate)
                    {
                    var ExecutionTimeSpan = EndDate - StartDate;
                    var Duration = ExecutionTimeSpan.Duration();
                    var DurationTime = Duration.ToString("hh\\:mm\\:ss\\.f");
                    return DurationTime;
                    }
                return string.Empty;
                }
            }

        /// <summary>
        /// Разность дат в виде строки с комментарием (время выполнения - xxx)
        /// </summary>
        public string TimeIntervalWithComment
            {
            get
                {
                var str = $"(время выполнения - {TimeInterval})";
                return str;
                }
            }

        /// <summary>
        /// Разность дат в виде строки с комментарием (время выполнения - xxx)
        /// </summary>
        /// <param name="nCount">Количество вызовов</param>
        public string GetTimeIntervalWithComment(int nCount)
            {
            string str = null;
            if (nCount <= 1 || (!(HasStartDate && HasEndDate)))
                {
                str = $"(время выполнения - {TimeInterval})";
                }
            else
                {
                var ExecutionTimeSpan = EndDate - StartDate;
                var avg = new TimeSpan(ExecutionTimeSpan.Ticks / nCount);
                var avgTime = avg.ToString();

                var avgSpeed = string.Empty;

                var TotalSeconds = ExecutionTimeSpan.TotalSeconds;
                if (TotalSeconds > 0)
                    {
                    var avgSpeedDouble = nCount / TotalSeconds;
                    avgSpeed = avgSpeedDouble.ToString("0.00");
                    }

                if (TotalSeconds == 0)
                    {
                    str = $"(время выполнения - {TimeInterval}, avg = {avgTime})";
                    }
                else
                if (TotalSeconds > 0)
                    {
                    str = $"(время выполнения - {TimeInterval}, avg = {avgTime} , avgSpeed = {avgSpeed} в секунду)";
                    }
                }
            return str;
            }
        };
    }