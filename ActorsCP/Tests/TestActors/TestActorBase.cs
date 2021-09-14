using System;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Actors;

namespace ActorsCP.Tests.TestActors
    {
    /// <summary>
    /// Базовый класс тестового объекта
    /// </summary>
    public class TestActorBase : ActorBase
        {
        #region Конструкторы

        /// <summary>
        ///
        /// </summary>
        protected TestActorBase()
            {
            }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        public TestActorBase(string name) : base(name)
            {
            }

        #endregion Конструкторы

        #region Перегрузки

        protected override void DisposeManagedResources()
            {
            base.DisposeManagedResources();
            DisposeManagedResources_Called = true;
            }

        /// <summary>
        /// Освободить неуправляемые ресурсы
        /// </summary>
        protected override void DisposeUnmanagedResources()
            {
            base.DisposeUnmanagedResources();
            DisposeUnmanagedResources_Called = true;
            }

        /// <summary>
        /// Частичная реализация - инициализация логгера
        /// </summary>
        protected override void InternalInitLogger()
            {
            base.InternalInitLogger();
            InternalInitLogger_Called = true;
            }

        /// <summary>
        /// Запуск
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override Task<bool> InternalStartAsync()
            {
            InternalStartAsync_Called = true;
            return base.InternalStartAsync();
            }

        /// <summary>
        /// Остановка
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override Task<bool> InternalStopAsync()
            {
            InternalStopAsync_Called = true;
            return base.InternalStopAsync();
            }

        /// <summary>
        /// Перегружаемая функция для выполнения некоторых действий
        /// </summary>
        /// <returns>true если объект все выполнил успешно</returns>
        protected override Task<bool> InternalRunAsync()
            {
            InternalRunAsync_Called = true;
            return base.InternalRunAsync();
            }

        /// <summary>
        /// Метод вызывается до отправки сигнала OnActorTerminated и предназначен для очистки объекта
        /// от хранимых в нем временных данных. Также вызывается из Dispose()
        /// </summary>
        protected override Task<bool> InternalRunCleanupBeforeTerminationAsync(bool fromDispose)
            {
            InternalRunCleanupBeforeTerminationAsync_Called = true;
            return base.InternalRunCleanupBeforeTerminationAsync(fromDispose);
            }

        #endregion Перегрузки

        #region Свойства

        public bool DisposeManagedResources_Called
            {
            get;
            private set;
            }

        public bool DisposeUnmanagedResources_Called
            {
            get;
            private set;
            }

        public bool InternalInitLogger_Called
            {
            get;
            private set;
            }

        public bool InternalStartAsync_Called
            {
            get;
            private set;
            }

        public bool InternalStopAsync_Called
            {
            get;
            private set;
            }

        public bool InternalRunAsync_Called
            {
            get;
            private set;
            }

        public bool InternalRunCleanupBeforeTerminationAsync_Called
            {
            get;
            private set;
            }

        #endregion Свойства
        }
    }