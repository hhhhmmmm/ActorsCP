// #define DEBUG_ADD_TO_QUEUE
// #define DEBUG_PROCESSMESSAGEFROMQUEUELOOP

using System;
using System.Diagnostics;
using System.Threading;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;
using ActorsCP.Helpers;
using ActorsCP.Logger;
using ActorsCP.UtilityActors;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Буферизованный класс вьюпорта со всеми интерфейсами
    /// и буферизованным выводом
    /// </summary>
    public class QueueBufferedActorViewPortBase : ActorViewPortBase, IMessageChannel
        {
        #region Приватные мемберы

        /// <summary>
        /// Очередь сообщения
        /// </summary>
        private QueueBufferT<ViewPortItem> _queue;

        /// <summary>
        /// Обработчик элемента ViewPortItem
        /// </summary>
        private IViewPortItemProcessor _iViewPortItemProcessor;

        #endregion Приватные мемберы

        #region Свойства

        /// <summary>
        /// Логгер
        /// </summary>
        protected IActorLogger Logger
            {
            get
                {
                return GlobalActorLogger.GetInstance();
                }
            }

        /// <summary>
        /// Внутренняя статистика буфера
        /// </summary>
        public QueueBufferStatistics Statistics
            {
            get
                {
                return _queue.Statistics;
                }
            }

        #endregion Свойства

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            if (!IsTerminated)
                {
                Terminate();
                }

            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable

        #region Очередь

        /// <summary>
        /// Установить обработчик IViewPortItemProcessor
        /// </summary>
        /// <param name="iViewPortItemProcessor">Обработчик элемента ViewPortItem</param>
        public void SetIViewPortItemProcessor(IViewPortItemProcessor iViewPortItemProcessor)
            {
            _iViewPortItemProcessor = iViewPortItemProcessor;
            }

        #endregion Очередь

        #region Инициализация/Завершение

        /// <summary>
        /// Инициализация вьюпорта
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        protected override void InternalInit(string additionalText)
            {
            if (IsInitialized)
                {
                return;
                }

            _queue = new QueueBufferT<ViewPortItem>(ProcessExtractedMessage);

            base.InternalInit(additionalText);
            }

        /// <summary>
        /// Завершение вьюпорта
        /// </summary>
        protected override async void InternalTerminate()
            {
            if (IsTerminated)
                {
                return;
                }

            await _queue.TerminateAsync();
            _iViewPortItemProcessor = null;

            base.InternalTerminate();
            }

        #endregion Инициализация/Завершение

        #region Добавление сообщений

        /// <summary>
        /// Добавить данные в очередь на обработку
        /// </summary>
        /// <param name="viewPortItem"></param>
        /// <returns></returns>
        public void Add(ViewPortItem viewPortItem)
            {
            if (viewPortItem == null)
                {
                throw new ArgumentNullException(nameof(viewPortItem), "data не может быть null");
                }

            if (_queue == null)
                {
                throw new InvalidOperationException("_queue == null");
                }

            _queue.Add(viewPortItem);

            _сurrentExecutionStatistics.BufferStatistics = _queue.Statistics;
            }

        #endregion Добавление сообщений

        #region Перегружаемые методы IActorEventsHandler

        /// <summary>
        /// События объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        protected override void InternalActor_Event(object sender, ActorEventArgs e)
            {
            var actor = sender as ActorBase;
            var viewPortItem = new ViewPortItem(actor, e);
            Add(viewPortItem);
            }

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        protected override void InternalActor_StateChangedEvent(object sender, ActorStateChangedEventArgs e)
            {
            var actor = sender as ActorBase;
            var viewPortItem = new ViewPortItem(actor, e);
            Add(viewPortItem);
            }

        #endregion Перегружаемые методы IActorEventsHandler

        #region Перегружаемые методы IActorBindViewPortHandler

        /// <summary>
        /// Вызывается, когда объект подписан на события или отписан от них
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        /// <param name="actorViewPortBoundEventArgs">Событие - объект привязан или отвязан</param>
        protected override void InternalActor_ViewPortBoundUnbound(ActorBase actor, ActorViewPortBoundEventArgs actorViewPortBoundEventArgs)
            {
            var viewPortItem = new ViewPortItem(actor, actorViewPortBoundEventArgs);
            Add(viewPortItem);
            }

        #endregion Перегружаемые методы IActorBindViewPortHandler

        #region Обработка сообщений

        /// <summary>
        /// Обработать извлеченное сообщение
        /// </summary>
        /// <param name="viewPortItem">Извлеченное сообщение</param>
        private void ProcessExtractedMessage(ViewPortItem viewPortItem)
            {
            if (viewPortItem == null)
                {
                throw new ArgumentNullException(nameof(viewPortItem), "viewPortItem не может быть null");
                }

            if (viewPortItem.ActorEventArgs == null)
                {
                throw new ArgumentNullException(nameof(viewPortItem), "viewPortItem.ActorEventArgs не может быть null");
                }

            _iViewPortItemProcessor?.ProcessViewPortItem(viewPortItem);

            _сurrentExecutionStatistics.BufferStatistics = _queue.Statistics;

            if (viewPortItem.ActorEventArgs is ActorStateChangedEventArgs)
                {
                ProcessAsActorStateChangedEventArgs(viewPortItem);
                }
            else
            if (viewPortItem.ActorEventArgs != null)
                {
                ProcessAsActorEventArgs(viewPortItem);
                }
            else
                {
                throw new Exception($"Непонятно как обрабатывать viewPortItem.ActorEventArgs = {viewPortItem.ActorEventArgs}");
                }

            viewPortItem.Dispose();
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

        #endregion Обработка сообщений

        #region Перегружаемые методы

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

        #region Реализация интерфейса IMessageChannel

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        public void RaiseDebug(string debugText)
            {
            if (NoOutMessages)
                {
                return;
                }

            var ea = new ActorActionEventArgs(debugText, ActorActionEventType.Debug);
            var viewPortItem = new ViewPortItem(EmptyActor.Value, ea);
            Add(viewPortItem); // RaiseDebug
            }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        public void RaiseMessage(string messageText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var ea = new ActorActionEventArgs(messageText, ActorActionEventType.Neutral);
            var viewPortItem = new ViewPortItem(EmptyActor.Value, ea);
            Add(viewPortItem); // RaiseMessage
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="warningText">Текст сообщения</param>
        public void RaiseWarning(string warningText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var ea = new ActorActionEventArgs(warningText, ActorActionEventType.Warning);
            var viewPortItem = new ViewPortItem(EmptyActor.Value, ea);
            Add(viewPortItem); // RaiseWarning
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="errorText">Текст сообщения об ошибке</param>
        public void RaiseError(string errorText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var ea = new ActorActionEventArgs(errorText, ActorActionEventType.Error);
            var viewPortItem = new ViewPortItem(EmptyActor.Value, ea);
            Add(viewPortItem); // RaiseError
            }

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void RaiseException(Exception exception)
            {
            if (NoOutMessages)
                {
                return;
                }
            var ea = new ActorExceptionEventArgs(exception);
            var viewPortItem = new ViewPortItem(EmptyActor.Value, ea);
            Add(viewPortItem); // RaiseException
            }

        #endregion Реализация интерфейса IMessageChannel
        }
    }