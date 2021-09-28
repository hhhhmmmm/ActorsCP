using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;
using ActorsCP.Helpers;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// элемент описывающий событие
    /// </summary>
    [DebuggerDisplay("VPI_{VPI}, ActorEventArgs = {ActorEventArgs}")]
    public sealed class ViewPortItem : DisposableImplementation<ViewPortItem>
        {
        #region Глобальные внутренние объекты

        /// <summary>
        /// Генератор последовательных номеров объектов
        /// </summary>
        private volatile static int s_VPI_global = 0;

        #endregion Глобальные внутренние объекты

        #region Приватные мемберы

        /// <summary>
        /// Уникальный последовательный номер объекта
        /// </summary>
        private readonly int _VPI = 0;

        #endregion Приватные мемберы

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sender">Объект-отправитель</param>
        /// <param name="actorEventArgs">Событие</param>
        public ViewPortItem(ActorBase sender, ActorEventArgs actorEventArgs)
            {
            _VPI = Interlocked.Increment(ref s_VPI_global); // последовательный номер объекта

            Sender = sender;
            ActorEventArgs = actorEventArgs;
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Уникальный последовательный номер объекта
        /// </summary>
        public int VPI
            {
            get
                {
                return _VPI;
                }
            }

        /// <summary>
        /// Объект-отправитель
        /// </summary>
        public ActorBase Sender
            {
            get;
            private set;
            }

        /// <summary>
        /// Событие
        /// </summary>
        public ActorEventArgs ActorEventArgs
            {
            get;
            private set;
            }

        #endregion Свойства

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            Sender = null;
            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable
        } // end class ViewPortItem
    } // end namespace ActorsCP.ViewPorts