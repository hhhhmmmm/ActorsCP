using System;
using System.Runtime.InteropServices;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Делегат который будет вызван по завершении жизни
    /// </summary>
    public delegate void GuActorDisposableTimeDelegate(string text);

    /// <summary>
    /// Счетчик времени для использования совместно с using
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    public class ActorDisposableTime : IDisposable
        {
        /// <summary>
        /// Название счетчика
        /// </summary>
        private readonly string m_CounterName;

        /// <summary>
        /// Счетчик
        /// </summary>
        private ActorTime m_ActorTime = default(ActorTime);

        /// <summary>
        /// Делегат который будет вызван по завершении жизни
        /// </summary>
        private readonly GuActorDisposableTimeDelegate m_Delegate;

        #region Свойства

        /// <summary>
        /// Время
        /// </summary>
        public ActorTime Time
            {
            get
                {
                return m_ActorTime;
                }
            }

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CounterName">Название счетчика</param>
        /// <param name="Delegate">Делегат который будет вызван по завершении жизни </param>
        public ActorDisposableTime(string CounterName, GuActorDisposableTimeDelegate Delegate)
            {
            if (CounterName == null)
                {
                throw new ArgumentNullException(nameof(CounterName), "CounterName не может быть null");
                }

            if (Delegate == null)
                {
                throw new ArgumentNullException(nameof(Delegate), "Delegate не может быть null");
                }

            m_CounterName = CounterName;
            m_Delegate = Delegate;
            m_ActorTime.SetStartDate();

            m_Delegate($"Начало {m_CounterName}");
            }

        #endregion Конструкторы

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Флаг для отслеживания того, что Dispose уже вызывался
        /// </summary>
        private bool m_Disposed;

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
            if (this.m_Disposed)
                {
                return;
                }

            // Если disposing=true, то освобождаем все (управляемые и неуправляемые) ресурсы
            if (disposing)
                {
                // Освобождаем управляемые ресурсы
                if (m_Delegate != null)
                    {
                    m_ActorTime.SetEndDate();
                    var str = string.Format("Конец {0} {1}", m_CounterName, m_ActorTime.TimeIntervalWithComment);
                    m_Delegate(str);
                    }
                }

            // Ставим флаг завершения
            m_Disposed = true;
            }

        #endregion Реализация интерфейса IDisposable
        }
    }