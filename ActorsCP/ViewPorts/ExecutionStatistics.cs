using System;
using System.Text;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Статистика выполнения
    /// </summary>
    public struct ExecutionStatistics
        {
        #region Счетчики

        /// <summary>
        /// Общее количество привязанных объектов
        /// </summary>
        public volatile int TotalBoundObjects;

        /// <summary>
        /// Общее количество отвязанных объектов
        /// </summary>
        public volatile int TotalUnboundObjects;

        /// <summary>
        /// Текущее количество привязанных объектов
        /// </summary>
        public volatile int BoundObjects;

        /// <summary>
        /// Общее количество отвязанных объектов
        /// </summary>
        public volatile int TerminatedObjects;

        /// <summary>
        /// Запущенных объектов
        /// </summary>
        public volatile int StartedObjects;

        /// <summary>
        /// Остановленных объектов
        /// </summary>
        public volatile int StoppedObjects;

        /// <summary>
        /// Работающих объектов
        /// </summary>
        public volatile int RunningObjects;

        /// <summary>
        /// Выброшенных исключений
        /// </summary>
        public volatile int Exceptions;

        /// <summary>
        /// Ошибок
        /// </summary>
        public volatile int Errors;

        #endregion Счетчики

        #region Перегрузки

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            {
            if (obj is ExecutionStatistics)
                {
                ExecutionStatistics c = (ExecutionStatistics)obj;
                return
                    TotalBoundObjects == c.TotalBoundObjects &&
                    TotalUnboundObjects == c.TotalUnboundObjects &&
                    BoundObjects == c.BoundObjects &&
                    TerminatedObjects == c.TerminatedObjects &&
                    RunningObjects == c.RunningObjects &&
                    Exceptions == c.Exceptions &&
                    Errors == c.Errors
                    ;
                }

            return false;
            }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            {
            var hash =
                TotalBoundObjects.GetHashCode() ^
                TotalUnboundObjects.GetHashCode() ^
                BoundObjects.GetHashCode() ^
                TerminatedObjects.GetHashCode() ^
                Exceptions.GetHashCode() ^
                RunningObjects.GetHashCode() ^
                Errors.GetHashCode();
            return hash;
            }

        #endregion Перегрузки

        #region Свойства

        /// <summary>
        /// Статистика в виде текста
        /// </summary>
        public string TextStatistics
            {
            get
                {
                var nl = Environment.NewLine;
                var sb = new StringBuilder();
                sb.Append($"Рабочих объектов:" + nl);
                sb.Append($"привязано - {TotalBoundObjects}, " + nl);
                sb.Append($"работают - {RunningObjects}, " + nl);
                sb.Append($"завершено - {TerminatedObjects}, " + nl);
                sb.Append($"исключений - {Exceptions}, " + nl);
                sb.Append($"ошибок - {Errors}, " + nl);
                sb.Append($"отвязано - {TotalUnboundObjects}, " + nl);
                sb.Append($"запущено - {StartedObjects}, " + nl);
                sb.Append($"остановлено - {StoppedObjects}" + nl);

                return sb.ToString();
                }
            }

        /// <summary>
        /// Процент выполнения
        /// </summary>
        public string PercentCompleted
            {
            get
                {
                if (TotalBoundObjects == 0)
                    {
                    return "0% - ";
                    }
                string str = string.Format("{0}% - ", (TerminatedObjects * 100) / TotalBoundObjects);
                return str;
                }
            }

        /// <summary>
        /// Завершено 100%
        /// </summary>
        public bool PercentCompleted100
            {
            get
                {
                if (TerminatedObjects == TotalBoundObjects)
                    {
                    return true;
                    }
                return false;
                }
            }

        /// <summary>
        /// Все события собраны
        /// </summary>
        public bool AllEventCollected
            {
            get
                {
                if (TotalBoundObjects == 0)
                    {
                    return false;
                    }
                return TotalBoundObjects == TerminatedObjects;
                }
            }

        #endregion Свойства

        #region Операторы

        /// <summary>
        /// Оператор равенства ==
        /// </summary>
        /// <param name="m1">Первый ExecutionStatistics</param>
        /// <param name="m2">Второй ExecutionStatistics</param>
        /// <returns>true если равны</returns>
        public static bool operator ==(ExecutionStatistics m1, ExecutionStatistics m2)
            {
            return Helpers.IEquatableHelper.OperatorEquals<ExecutionStatistics>(m1, m2);
            }

        /// <summary>
        /// Оператор неравенства !=
        /// </summary>
        /// <param name="m1">Первый ExecutionStatistics</param>
        /// <param name="m2">Второй ExecutionStatistics</param>
        /// <returns>true если не равны</returns>
        public static bool operator !=(ExecutionStatistics m1, ExecutionStatistics m2)
            {
            return Helpers.IEquatableHelper.OperatorNotEquals<ExecutionStatistics>(m1, m2);
            }

        #endregion Операторы

        /// <summary>
        /// Проверить корректность собранных значений счетчиков
        /// </summary>
        public void ValidateStatistics()
            {
            if (!AllEventCollected)
                {
                throw new Exception("Не все события собраны - AllEventCollected = false");
                }

            if (TotalBoundObjects != TotalUnboundObjects)
                {
                var str = $"TotalBoundObjects!= TotalUnboundObjects({ TotalBoundObjects } != {TotalUnboundObjects})";
                throw new Exception(str);
                }

            if (TotalBoundObjects != TotalUnboundObjects)
                {
                var str = $"TotalBoundObjects!= TotalUnboundObjects({ TotalBoundObjects } != {TotalUnboundObjects})";
                throw new Exception(str);
                }

            if (StartedObjects != StoppedObjects)
                {
                var str = $"StartedObjects!= StoppedObjects({ StartedObjects } != {StoppedObjects})";
                throw new Exception(str);
                }

            if (RunningObjects != 0)
                {
                throw new Exception($"Не все объекты остановлены - RunningObjects = {RunningObjects}");
                }
            }
        } // end struct ExecutionStatistics
    } // end namespace ActorsCP.ViewPorts