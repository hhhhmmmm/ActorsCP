using System;
using System.Collections.Concurrent;
using System.Text;
using ActorsCP.Actors.Events;
using ActorsCP.ViewPorts;
using Timer = System.Windows.Forms.Timer;

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

        #region Свойства

        /// <summary>
        /// Отображать элементы используя дополнительную очередь
        /// </summary>
        public bool ProcessOnTheQueue
            {
            get;
            set;
            } = true;

        #endregion Свойства

        #region Создание/удаление вьюпорта

        /// <summary>
        /// Таймер отображения на экран
        /// </summary>
        private Timer _queueTimer;

        /// <summary>
        /// Очередь сообщений для обработки на экране
        /// </summary>
        private ConcurrentQueue<ViewPortItem> _queue;

        /// <summary>
        /// Создать вьюпорт
        /// </summary>
        private void CreateViewPort()
            {
            #region Создание таймера отображения на экран

            if (ProcessOnTheQueue)
                {
                _queueTimer = new Timer
                    {
                    Enabled = true,
                    Interval = 40
                    };
                _queueTimer.Tick += OnQueueTimer;
                _queueTimer.Start();

                _queue = new ConcurrentQueue<ViewPortItem>();
                }

            #endregion Создание таймера отображения на экран

            _viewPort = new QueueBufferedActorViewPortBase();
            _viewPort.SetIViewPortItemProcessor(this);

            if (ProcessOnTheQueue)
                {
                _viewPort.DisposeViewPortItemAfterProcessing = false;
                }

            _viewPort.Init();
            Actor.BindViewPort(_viewPort);
            }

        /// <summary>
        /// Событие таймера отображения на экран
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQueueTimer(object sender, EventArgs e)
            {
            _childControl.SuspendLayout();
            for (var i = 0; i < 100; i++)
                {
                var bres = _queue.TryDequeue(out ViewPortItem viewPortItem);
                if (!bres)
                    {
                    break;
                    }

                InternalProcessViewPortItem(viewPortItem);
                viewPortItem.Dispose();
                }
            _childControl.ResumeLayout();
            }

        /// <summary>
        /// Удалить вьюпорт
        /// </summary>
        private void TerminateViewPort()
            {
            if (_queueTimer != null)
                {
                _queueTimer.Stop();
                _queueTimer.Dispose();
                }
            _viewPort?.Terminate();
            }

        #endregion Создание/удаление вьюпорта

        #region Реализация интерфейса IViewPortItemProcessor

        /// <summary>
        /// Обработать элемент типа ViewPortItem
        /// </summary>
        /// <param name="viewPortItem">элемент типа ViewPortItem</param>
        public void ProcessViewPortItem(ViewPortItem viewPortItem)
            {
            if (ProcessOnTheQueue)
                {
                _queue.Enqueue(viewPortItem);
                }
            else
                {
                InternalProcessViewPortItem(viewPortItem);
                }
            }

        #endregion Реализация интерфейса IViewPortItemProcessor

        /// <summary>
        /// Обработать элемент типа ViewPortItem
        /// </summary>
        /// <param name="viewPortItem">элемент типа ViewPortItem</param>
        private void InternalProcessViewPortItem(ViewPortItem viewPortItem)
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

            _viewPort.Increment_СurrentExecutionStatistics_ViewPortDisplayed();
            }

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