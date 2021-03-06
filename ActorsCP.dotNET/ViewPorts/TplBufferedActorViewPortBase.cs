using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;
using ActorsCP.Helpers;
using ActorsCP.UtilityActors;
using ActorsCP.ViewPorts;

namespace ActorsCP.dotNET.ViewPorts
    {
    /// <summary>
    /// Базовый класс вьюпорта со всеми интерфейсами
    /// и буферизованным выводом
    /// </summary>
    public class TplBufferedActorViewPortBase : ActorViewPortBase, IMessageChannel
        {
        #region Приватные мемберы

        /// <summary>
        /// Буфер сообщений
        /// </summary>
        private BufferBlock<ViewPortItem> _tplDataFlowDataBufferBlock;

        /// <summary>
        /// Поток действия
        /// </summary>
        private ActionBlock<ViewPortItem> _tplDataFlowDataActionBlock;

        /// <summary>
        /// Очередь сообщения как альтернатива Tpl
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

        #endregion Приватные мемберы

        #region Свойства

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
        /// Цикл обработки сообщений
        /// </summary>
        private void ProcessMessageFromQueueLoop()
            {
            while (true)
                {
                _queueSemaphoreSlim.Wait(QueueTimeout);

                if (_queue.Count > 0 && _queue.TryDequeue(out ViewPortItem viewPortItem))
                    {
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
            _queueTask?.Dispose();
            _queueTask = null;
            _queueSemaphoreSlim?.Dispose();
            _queueSemaphoreSlim = null;

            if (_queue.Count != 0)
                {
                throw new Exception($"В очереди осталось {_queue.Count} сообщений");
                }

            _queue = null;
            }

        #endregion Очередь

        #region TPL

        /// <summary>
        /// Инициализация Tpl
        /// </summary>
        private void InternalInitAsTpl()
            {
            var dataflowLinkOptions = new DataflowLinkOptions
                {
                PropagateCompletion = true
                };

            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions
                {
                MaxDegreeOfParallelism = 1
                };

            var dataflowBlockOptions = new DataflowBlockOptions()
                {
                // MaxMessagesPerTask = 1
                };

            _tplDataFlowDataBufferBlock = new BufferBlock<ViewPortItem>(dataflowBlockOptions);
            _tplDataFlowDataActionBlock = new ActionBlock<ViewPortItem>(TplProcessExtractedMessage, executionDataflowBlockOptions);
            _tplDataFlowDataBufferBlock.LinkTo(_tplDataFlowDataActionBlock, dataflowLinkOptions);

            _tplDataFlowDataBufferBlock.Completion.ContinueWith(TplDataFlow_DataBufferBlock_Completion);
            _tplDataFlowDataActionBlock.Completion.ContinueWith(TplDataFlow_ActionBlock_Completion);
            }

        /// <summary>
        /// Завершение Tpl
        /// </summary>
        private void InternalTerminateAsTpl()
            {
            _tplDataFlowDataBufferBlock.Complete();
            _tplDataFlowDataBufferBlock.Completion.Wait();
            _tplDataFlowDataBufferBlock = null;

            _tplDataFlowDataActionBlock.Complete();
            _tplDataFlowDataActionBlock.Completion.Wait();
            _tplDataFlowDataActionBlock = null;
            }

        #region Завершители Tpl

        /// <summary>
        /// Завершение работы буфера
        /// </summary>
        /// <param name="task">Задача завершения</param>
        private void TplDataFlow_DataBufferBlock_Completion(Task task)
            {
            }

        /// <summary>
        /// Завершение работы актора
        /// </summary>
        /// <param name="task">Задача завершения</param>
        private void TplDataFlow_ActionBlock_Completion(Task task)
            {
            }

        #endregion Завершители Tpl

        /// <summary>
        /// Обработать сообщение
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        private void TplProcessExtractedMessage(ViewPortItem viewPortItem)
            {
            ProcessExtractedMessage(viewPortItem);
            }

        #endregion TPL

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

            if (UseQueueForBuffering)
                {
                InternalInitAsQueue();
                }
            else
                {
                InternalInitAsTpl();
                }

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

            if (UseQueueForBuffering)
                {
                InternalTerminateAsQueue();
                }
            else
                {
                InternalTerminateAsTpl();
                }

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
                throw new ArgumentNullException($"{nameof(viewPortItem)} не может быть null");
                }

            Interlocked.Increment(ref _сurrentExecutionStatistics.BufferStatistics.AddedMessages);

            if (UseQueueForBuffering)
                {
                if (_queue == null)
                    {
                    throw new InvalidOperationException("_queue == null");
                    }

                _queue.Enqueue(viewPortItem);
                _queueSemaphoreSlim.Release();
                }
            else
                {
                if (_tplDataFlowDataBufferBlock == null)
                    {
                    throw new InvalidOperationException("_tplDataFlowDataBufferBlock == null");
                    }

                _tplDataFlowDataBufferBlock.Post(viewPortItem); // await SendAsync(data) - медленно
                }
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

            Interlocked.Increment(ref _сurrentExecutionStatistics.BufferStatistics.ProcessedMessages);

            if (viewPortItem.ActorEventArgs is ActorStateChangedEventArgs)
                {
                ProcessAsActorStateChangedEventArgs(viewPortItem);
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
            Add(viewPortItem);
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
            Add(viewPortItem);
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
            Add(viewPortItem);
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
            Add(viewPortItem);
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
            Add(viewPortItem);
            }

        #endregion Реализация интерфейса IMessageChannel
        }
    }