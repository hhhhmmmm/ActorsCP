using System;
using System.Text;

using ActorsCP.Helpers;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Статистика выполнения
    /// </summary>
    public struct ExecutionStatistics
        {
        #region Счетчики

        #region Обычный вьюпорт

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

        /// <summary>
        /// Измениось состояние
        /// </summary>
        public volatile int StateChanged;

        #endregion Обычный вьюпорт

        #region Буфер

        /// <summary>
        /// Статистика буфера
        /// </summary>
        public QueueBufferStatistics BufferStatistics;

        #endregion Буфер

        /// <summary>
        /// Количество объектов показанных на экране
        /// </summary>
        public volatile int ViewPortDisplayed;

        #endregion Счетчики

        #region Перегрузки

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            {
            if (obj is ExecutionStatistics c)
                {
                return
                    TotalBoundObjects == c.TotalBoundObjects &&
                    TotalUnboundObjects == c.TotalUnboundObjects &&
                    BoundObjects == c.BoundObjects &&
                    TerminatedObjects == c.TerminatedObjects &&
                    RunningObjects == c.RunningObjects &&
                    Exceptions == c.Exceptions &&
                    Errors == c.Errors &&
                    StateChanged == c.StateChanged &&
                    BufferStatistics.AddedMessages == c.BufferStatistics.AddedMessages &&
                    BufferStatistics.ProcessedMessages == c.BufferStatistics.ProcessedMessages &&
                    ViewPortDisplayed == c.ViewPortDisplayed
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
                Errors.GetHashCode() ^
                StateChanged.GetHashCode();
            return hash;
            }

        #endregion Перегрузки

        #region Свойства

        /// <summary>
        /// Добавить кусок статистики
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="prefix">Префикс</param>
        /// <param name="name">Название</param>
        /// <param name="value">Значение</param>
        private static void AddStatistics(StringBuilder sb, string prefix, string name, int value)
            {
            if (value == 0)
                {
                return;
                }
            if (!string.IsNullOrEmpty(prefix))
                {
                sb.Append(prefix);
                sb.Append(' ');
                }
            sb.Append(name);
            sb.Append(" - ");
            sb.Append(value);
            }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string GetStatistics()
            {
            var sb = new StringBuilder();
            sb.Append("Рабочих объектов: ");

            AddStatistics(sb, "", "привязано", TotalBoundObjects);
            AddStatistics(sb, ",", "запущено", StartedObjects);
            AddStatistics(sb, ",", "работают", RunningObjects);
            AddStatistics(sb, ",", "остановлено", StoppedObjects);
            AddStatistics(sb, ",", "завершено", TerminatedObjects);
            AddStatistics(sb, ",", "отвязано", TotalUnboundObjects);

            AddStatistics(sb, ",", "исключений", Exceptions);
            AddStatistics(sb, ",", "ошибок", Errors);

            AddStatistics(sb, ",", "изменений состояния", StateChanged);
            AddStatistics(sb, ",", "Б: добавлено", BufferStatistics.AddedMessages);
            AddStatistics(sb, ",", "Б: обработано", BufferStatistics.ProcessedMessages);
            AddStatistics(sb, ",", "В: отображено", ViewPortDisplayed);

            return sb.ToString();
            }

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
                sb.Append($"изменений состояния - {StateChanged}" + nl);
                sb.Append($"====================================" + nl);

                if (BufferStatistics.AddedMessages != 0)
                    {
                    sb.AppendLine($"Буфер сообщений: добавлено сообщений  - {BufferStatistics.AddedMessages}");
                    }

                if (BufferStatistics.ProcessedMessages != 0)
                    {
                    sb.AppendLine($"Буфер сообщений: обработано сообщений - {BufferStatistics.ProcessedMessages}");
                    }

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
                if (TotalBoundObjects == 0) // не было ни одного объекта
                    {
                    return true;
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

            if (BufferStatistics.AddedMessages != BufferStatistics.ProcessedMessages)
                {
                var str = $"BufferStatistics.AddedMessages!= BufferStatistics.ProcessedMessages({ BufferStatistics.AddedMessages } != {BufferStatistics.ProcessedMessages})";
                throw new Exception(str);
                }
            }
        } // end struct ExecutionStatistics
    } // end namespace ActorsCP.ViewPorts