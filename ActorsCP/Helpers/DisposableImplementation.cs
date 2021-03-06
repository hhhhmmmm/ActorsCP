using System;

namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Вспомогательный класс - реализация интерфейса IDisposable
    /// Производные классы должны переопределить void Dispose(bool disposing)
    /// </summary>
    /// <typeparam name="T">Имя типа класса</typeparam>

    public class DisposableImplementation<T> : IDisposable where T : class, IDisposable
        {
        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Свойство для флага _disposed
        /// </summary>
        protected bool Disposed
            {
            get;
            private set;
            }

        /// <summary>
        /// Функция которая вызывается первой перед вызовом DisposeManagedResources();
        /// </summary>
        private Action _preDisposeHandler;

        /// <summary>
        /// Установить функцию которую нужно вызвать при вызове Dispose() первой
        /// </summary>
        /// <param name="PreDisposeHandler"></param>
        protected void SetPreDisposeHandler(Action PreDisposeHandler)
            {
            if (_preDisposeHandler != null)
                {
                throw new InvalidOperationException("Повторный вызов SetPreDisposeHandler()");
                }
            _preDisposeHandler = PreDisposeHandler;
            }

        /// <summary>
        /// Реализация Dispose()
        /// </summary>
        public void Dispose()
            {
            Dispose(true);

            // Объект будет освобожден при помощи метода Dispose, следовательно необходимо вызвать
            // GC.SupressFinalize чтобы убрать этот объект из очереди финализатора и предотвратить
            // вызов финализатора во второй раз
            GC.SuppressFinalize(this);
            }

        /// <summary>
        /// Метод Dispose(bool disposing) выполняется в двух различных вариантах: Если
        /// disposing=true, то этот метод вызывается (прямо или косвенно) из пользовательского кода.
        /// Управляемые и неуправляемые ресурсы могут быть освобождены. Если disposing=false, то этот
        /// метод вызывается рантаймом из финализатора и могут освобождаться только неуправляемые ресурсы.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
            {
            // Проверка что Dispose(bool) уже вызывался
            if (this.Disposed)
                {
                return;
                }

            _preDisposeHandler?.Invoke();

            // Если disposing=true, то освобождаем все (управляемые и неуправляемые) ресурсы
            if (disposing)
                {
                // Освобождаем управляемые ресурсы
                DisposeManagedResources();
                }

            // Вызываем соответствующие методы для освобождения неуправляемых ресурсов
            DisposeUnmanagedResources();

            // Ставим флаг завершения
            Disposed = true;

            // base.Dispose(disposing);
            }

        #endregion Реализация интерфейса IDisposable

        #region Реализация вызываемых методов интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected virtual void BeforeDispose()
            {
            }

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected virtual void DisposeManagedResources()
            {
            }

        /// <summary>
        /// Освободить неуправляемые ресурсы
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
            {
            }

        #endregion Реализация вызываемых методов интерфейса IDisposable
        }
    }