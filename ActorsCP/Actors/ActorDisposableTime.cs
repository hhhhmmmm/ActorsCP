using System;
using System.Runtime.InteropServices;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Счетчик времени для использования совместно с using
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class ActorDisposableTime : IDisposable
        {
        /// <summary>
        /// Название счетчика
        /// </summary>
        private readonly string _counterName;

        /// <summary>
        /// Счетчик
        /// </summary>
        private ActorTime _actorTime = default;

        /// <summary>
        /// Делегат который будет вызван по завершении жизни
        /// </summary>
        private readonly Action<string> _delegate;

        #region Свойства

        /// <summary>
        /// Подавить вывод
        /// </summary>
        public bool SuppressOutput
            {
            get;
            private set;
            }

        /// <summary>
        /// Время
        /// </summary>
        public ActorTime Time
            {
            get
                {
                return _actorTime;
                }
            }

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="counterName">Название счетчика</param>
        /// <param name="_delegate">Делегат который будет вызван по завершении жизни</param>
        /// <param name="suppressOutput">Подавить вывод</param>
        public ActorDisposableTime(string counterName, Action<string> _delegate, bool suppressOutput = false)
            {
            if (counterName == null)
                {
                throw new ArgumentNullException(nameof(counterName), "CounterName не может быть null");
                }

            if (_delegate == null)
                {
                throw new ArgumentNullException(nameof(Delegate), "Delegate не может быть null");
                }

            SuppressOutput = suppressOutput;
            _counterName = counterName;
            this._delegate = _delegate;
            _actorTime.SetStartDate();

            if (!SuppressOutput)
                {
                this._delegate($"Начало {_counterName}");
                }
            }

        #endregion Конструкторы

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Флаг для отслеживания того, что Dispose уже вызывался
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Реализация Dispose()
        /// </summary>
        public void Dispose()
            {
            Dispose(true);
            // Объект будет освобожден при помощи метода Dispose, следовательно необходимо вызвать GC.SupressFinalize чтобы убрать этот объект
            // из очереди финализатора и предотвратить вызов финализатора во второй раз
            GC.SuppressFinalize(this);
            }

        /// <param name="disposing">true если управляемые ресурсы должны быть освобождены, иначе - false</param>
        private void Dispose(bool disposing)
            {
            // Проверка что Dispose(bool) уже вызывался
            if (this._disposed)
                {
                return;
                }

            // Если disposing=true, то освобождаем все (управляемые и неуправляемые) ресурсы
            if (disposing)
                {
                // Освобождаем управляемые ресурсы
                if (_delegate != null)
                    {
                    _actorTime.SetEndDate();
                    if (!SuppressOutput)
                        {
                        var str = $"Конец {_counterName} { _actorTime.TimeIntervalWithComment}";
                        _delegate(str);
                        }
                    }
                }

            // Ставим флаг завершения
            _disposed = true;
            }

        #endregion Реализация интерфейса IDisposable
        }
    }