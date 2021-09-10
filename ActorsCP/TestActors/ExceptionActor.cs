﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Actors;

namespace ActorsCP.TestActors
    {
    /// <summary>
    /// Выбрасывает исключения при разных условиях
    /// </summary>
    public class ExceptionActor : ActorBase
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parameter"></param>
        public ExceptionActor()
            {
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        ///
        /// </summary>
        public bool ExceptionOnStart
            {
            get;
            set;
            }

        /// <summary>
        ///
        /// </summary>
        public bool ExceptionOnStop
            {
            get;
            set;
            }

        /// <summary>
        ///
        /// </summary>
        public bool ExceptionOnRun
            {
            get;
            set;
            }

        /// <summary>
        ///
        /// </summary>
        public bool ExceptionOnRunCleanupBeforeTerminationAsync
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
            if (ExceptionOnStart)
                {
                throw new Exception();
                }
            return await base.InternalStartAsync();
            }

        /// <summary>
        ///
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override async Task<bool> InternalStopAsync()
            {
            if (ExceptionOnStop)
                {
                throw new Exception();
                }
            return await base.InternalStopAsync();
            }

        /// <summary>
        ///
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override async Task<bool> InternalRunAsync()
            {
            if (ExceptionOnRun)
                {
                throw new Exception();
                }
            return await base.InternalRunAsync();
            }

        /// <summary>
        /// Метод вызывается до отправки сигнала OnActorTerminated и предназначен для очистки объекта
        /// от хранимых в нем временных данных. Также вызывается из Dispose()
        /// </summary>
        protected override Task<bool> InternalRunCleanupBeforeTerminationAsync()
            {
            if (ExceptionOnRunCleanupBeforeTerminationAsync)
                {
                throw new Exception();
                }
            return base.InternalRunCleanupBeforeTerminationAsync();
            }
        } // end class ExceptionActor
    } // end namespace ActorsCP.TestActors