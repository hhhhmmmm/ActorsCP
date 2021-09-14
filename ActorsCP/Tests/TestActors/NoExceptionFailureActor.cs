using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Actors;

namespace ActorsCP.Tests.TestActors
    {
    /// <summary>
    /// Не выбрасывает исключения при разных условиях, но завершается с ошибкой
    /// </summary>
    public class NoExceptionFailureActor : TestActorBase
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public NoExceptionFailureActor()
            {
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        ///
        /// </summary>
        public bool FailureOnStart
            {
            get;
            set;
            }

        /// <summary>
        ///
        /// </summary>
        public bool FailureOnStop
            {
            get;
            set;
            }

        /// <summary>
        ///
        /// </summary>
        public bool FailureOnRun
            {
            get;
            set;
            }

        /// <summary>
        ///
        /// </summary>
        public bool FailureOnRunCleanupBeforeTerminationAsync
            {
            get;
            set;
            }

        #endregion Свойства

        /// <summary>
        /// Запуск
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override async Task<bool> InternalStartAsync()
            {
            if (FailureOnStart)
                {
                SetAnErrorOccurred();
                }
            return await base.InternalStartAsync();
            }

        /// <summary>
        ///
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override async Task<bool> InternalStopAsync()
            {
            if (FailureOnStop)
                {
                SetAnErrorOccurred();
                }
            return await base.InternalStopAsync();
            }

        /// <summary>
        ///
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override async Task<bool> InternalRunAsync()
            {
            if (FailureOnRun)
                {
                SetAnErrorOccurred();
                }
            return await base.InternalRunAsync();
            }

        /// <summary>
        /// Метод вызывается до отправки сигнала OnActorTerminated и предназначен для очистки объекта
        /// от хранимых в нем временных данных. Также вызывается из Dispose()
        /// </summary>
        /// <param name="fromDispose">Вызов из Dispose()</param>
        protected override Task<bool> InternalRunCleanupBeforeTerminationAsync(bool fromDispose)
            {
            if (FailureOnRunCleanupBeforeTerminationAsync)
                {
                SetAnErrorOccurred();
                }
            return base.InternalRunCleanupBeforeTerminationAsync(fromDispose);
            }
        } // end class ExceptionActor
    }