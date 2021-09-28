// #define DEBUG_ADD_TO_QUEUE
// #define DEBUG_PROCESSMESSAGEFROMQUEUELOOP

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
        private ConcurrentQueue<ViewPortItem> _queue;

        /// <summary>
        /// Семафор для очереди
        /// </summary>
        private SemaphoreSlim _queueSemaphoreSlim;

        /// <summary>
        /// Вьюпорт находится в состоянии завершения
        /// </summary>
        private bool _isTerminating;

        /// <summary>
        /// Задача-обработчик очереди сообщений
        /// </summary>
        private Task _queueTask;

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
        /// Использовать очередь для буфера
        /// </summary>
        public bool UseQueueForBuffering
            {
            get;
            set;
            } = true;

        /// <summary>
        /// Таймаут ожидания очереди, ms
        /// </summary>
        private int _queueTimeout = 100;

        /// <summary>
        /// Таймаут ожидания очереди, ms
        /// </summary>
        public int QueueTimeout
            {
            get
                {
                return _queueTimeout;
                }
            set
                {
                if (value < 0)
                    {
                    throw new ArgumentException("Значение должно быть больше 0");
                    }
                _queueTimeout = value;
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

        /// <summary>
        /// Цикл обработки сообщений
        /// </summary>
        private void ProcessMessageFromQueueLoop()
            {
            while (true)
                {
                _queueSemaphoreSlim.Wait(QueueTimeout);

                if ((!(_queue.IsEmpty)) && _queue.TryDequeue(out ViewPortItem viewPortItem))
                    {
                    //#if DEBUG_PROCESSMESSAGEFROMQUEUELOOP
                    //                    var str = $"ProcessMessageFromQueueLoop(VPI_{viewPortItem.VPI}):viewPortItem (AEA_{viewPortItem.ActorEventArgs.AEA}): {viewPortItem.ActorEventArgs.ToString() }";
                    //                    Logger.LogDebug(str);
                    //                    Debug.WriteLine(str);
                    //#endif // DEBUG_PROCESSMESSAGEFROMQUEUELOOP

                    ProcessExtractedMessage(viewPortItem);
                    }
                else
                    {
                    if (_isTerminating)
                        {
                        if (_queue.IsEmpty)
                            {
                            return;
                            }
                        }
                    }
                }
            }

        /// <summary>
        ///
        /// </summary>
        private void InternalInitAsQueue()
            {
            _queue = new ConcurrentQueue<ViewPortItem>();
            _queueSemaphoreSlim = new SemaphoreSlim(1, int.MaxValue);
            _queueTask = Task.Run(() => { ProcessMessageFromQueueLoop(); });
            }

        /// <summary>
        ///
        /// </summary>
        private void InternalTerminateAsQueue()
            {
            _queueTask.Wait();
            _queueTask.Dispose();
            _queueTask = null;
            _queueSemaphoreSlim?.Dispose();
            _queueSemaphoreSlim = null;

            if (!_queue.IsEmpty)
                {
                throw new Exception($"В очереди осталось {_queue.Count} сообщений");
                }

            _queue = null;

            _iViewPortItemProcessor = null;
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

            InternalInitAsQueue();

            base.InternalInit(additionalText);
            }

        /// <summary>
        /// Завершение вьюпорта
        /// </summary>
        protected override void InternalTerminate()
            {
            if (IsTerminated)
                {
                return;
                }

            _isTerminating = true;

            InternalTerminateAsQueue();

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

            Interlocked.Increment(ref _сurrentExecutionStatistics.BufferedAddedMessages);

            if (_queue == null)
                {
                throw new InvalidOperationException("_queue == null");
                }

            _queue.Enqueue(viewPortItem);
            _queueSemaphoreSlim.Release();

            //#if DEBUG_ADD_TO_QUEUE
            //            var str = $"Add(VPI_{viewPortItem.VPI}, AEA_{viewPortItem.ActorEventArgs.AEA} )";
            //            Logger.LogDebug(str);
            //            Debug.WriteLine(str);
            //#endif // DEBUG_ADD_TO_QUEUE
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

            Interlocked.Increment(ref _сurrentExecutionStatistics.BufferedProcessedMessages);

            _iViewPortItemProcessor?.ProcessViewPortItem(viewPortItem);

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