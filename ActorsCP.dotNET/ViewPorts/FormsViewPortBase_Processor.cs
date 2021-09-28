using System;
using System.Text;

using ActorsCP.Actors.Events;
using ActorsCP.ViewPorts;

namespace ActorsCP.dotNET.ViewPorts
    {
    /// <summary>
    /// Обработчик сообщений очереди
    /// </summary>
    partial class FormsViewPortBase : IViewPortItemProcessor
        {
        #region Приватные мемберы

        /// <summary>
        /// Экземпляр вьюпорта
        /// </summary>
        private QueueBufferedActorViewPortBase _viewPort;

        #endregion Приватные мемберы

        #region Создание/удаление вьюпорта

        /// <summary>
        /// Создать вьюпорт
        /// </summary>
        private void CreateViewPort()
            {
            _viewPort = new QueueBufferedActorViewPortBase();
            _viewPort.SetIViewPortItemProcessor(this);
            _viewPort.Init();
            Actor.BindViewPort(_viewPort);
            }

        /// <summary>
        /// Удалить вьюпорт
        /// </summary>
        private void TerminateViewPort()
            {
            // Actor.UnbindViewPort(_viewPort);
            _viewPort.Terminate();
            }

        #endregion Создание/удаление вьюпорта

        #region Реализация интерфейса IViewPortItemProcessor

        /// <summary>
        /// Обработать элемент типа ViewPortItem
        /// </summary>
        /// <param name="viewPortItem">элемент типа ViewPortItem</param>
        public void ProcessViewPortItem(ViewPortItem viewPortItem)
            {
            //#if DEBUG
            //            Logger.LogInfo($"FormsViewPortBase:ProcessViewPortItem(): {viewPortItem.ActorEventArgs}");
            //#endif // DEBUG

            if (viewPortItem.ActorEventArgs is ActorStateChangedEventArgs)
                {
                ProcessAsActorStateChangedEventArgs(viewPortItem);
                }
            else
            if (viewPortItem.ActorEventArgs is ActorViewPortBoundEventArgs)
                {
                ProcessAsActorViewPortBoundEventArgs(viewPortItem);
                }
            else
            if (viewPortItem.ActorEventArgs is ActorEventArgs)
                {
                ProcessAsActorEventArgs(viewPortItem);
                }
            else
                {
                throw new Exception($"Непонятно как обрабатывать viewPortItem.ActorEventArgs = {viewPortItem.ActorEventArgs}");
                }
            }

        #endregion Реализация интерфейса IViewPortItemProcessor

        /// <summary>
        /// Обработать как ActorViewPortBoundEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        private void ProcessAsActorViewPortBoundEventArgs(ViewPortItem viewPortItem)
            {
            InternalProcessAsActorViewPortBoundEventArgs(viewPortItem);
            }

        /// <summary>
        /// Обработать как ActorEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        private void ProcessAsActorEventArgs(ViewPortItem viewPortItem)
            {
            InternalProcessAsActorEventArgs(viewPortItem);
            }

        /// <summary>
        /// Обработать как ActorStateChangedEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        private void ProcessAsActorStateChangedEventArgs(ViewPortItem viewPortItem)
            {
            InternalProcessAsActorStateChangedEventArgs(viewPortItem);
            }

        #region Перегружаемые методы

        /// <summary>
        /// Обработать как ActorViewPortBoundEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected virtual void InternalProcessAsActorViewPortBoundEventArgs(ViewPortItem viewPortItem)
            {
            }

        /// <summary>
        /// Обработать как ActorEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected virtual void InternalProcessAsActorEventArgs(ViewPortItem viewPortItem)
            {
            }

        /// <summary>
        /// Обработать как ActorStateChangedEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected virtual void InternalProcessAsActorStateChangedEventArgs(ViewPortItem viewPortItem)
            {
            }

        #endregion Перегружаемые методы
        } // end class FormsViewPortBase
    } // end namespace ActorsCP.dotNET.ViewPorts