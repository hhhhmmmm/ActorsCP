#if DEBUG
// #define DEBUG_TPL_ERRORREPORTER
#endif // DEBUG_TPL_ERRORREPORTER

using System;
using System.Diagnostics;
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
    public class BufferedActorViewPortBase : ActorViewPortBase, IMessageChannel
        {
        #region Приватные мемберы

        private readonly ActorBase emptyActor = new EmptyActor();

        /// <summary>
        /// Буфер сообщений
        /// </summary>
        private BufferBlock<ViewPortItem> _tplDataFlowDataBufferBlock;

        /// <summary>
        /// Актор работы с ошибками
        /// </summary>
        private ActionBlock<ViewPortItem> _tplDataFlowDataActionBlock;

        #endregion Приватные мемберы

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public BufferedActorViewPortBase()
            {
            InitTplDataFlow();
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Tpl завершен
        /// </summary>
        public bool TplDataFlowInitialized
            {
            get;
            private set;
            }

        /// <summary>
        /// Tpl завершен
        /// </summary>
        public bool TplDataFlowTerminated
            {
            get;
            private set;
            }

        #endregion Свойства

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override async void DisposeManagedResources()
            {
            if (TplDataFlowInitialized && (!TplDataFlowTerminated))
                {
                await TerminateTplDataFlowAsync();
                }
            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable

        #region Инициализация/Завершение

        /// <summary>
        /// Инициализация TplDataFlow
        /// </summary>
        public void InitTplDataFlow()
            {
            if (_tplDataFlowDataBufferBlock != null)
                {
                return;
                }

            _tplDataFlowDataBufferBlock = new BufferBlock<ViewPortItem>();
            _tplDataFlowDataActionBlock = new ActionBlock<ViewPortItem>(TplDataFlowProcessAction);
            _tplDataFlowDataBufferBlock.LinkTo(_tplDataFlowDataActionBlock);

            _tplDataFlowDataBufferBlock.Completion.ContinueWith(TplDataFlow_DataBufferBlock_Completion);
            _tplDataFlowDataActionBlock.Completion.ContinueWith(TplDataFlow_ActionBlock_Completion);

            TplDataFlowInitialized = true;

#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine("InitTplDataFlow()");
#endif // DEBUG_TPL_ERRORREPORTER
            }

        /// <summary>
        /// Завершить TplDataFlow
        /// </summary>
        public async Task TerminateTplDataFlowAsync()
            {
#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine("TerminateTplDataFlowAsync() - начало");
#endif // DEBUG_TPL_ERRORREPORTER

            if (_tplDataFlowDataBufferBlock == null)
                {
                return;
                }

            _tplDataFlowDataBufferBlock.Complete();
            await _tplDataFlowDataBufferBlock.Completion;
            _tplDataFlowDataBufferBlock = null;

            _tplDataFlowDataActionBlock.Complete();
            await _tplDataFlowDataActionBlock.Completion;
            _tplDataFlowDataActionBlock = null;

            TplDataFlowTerminated = true;

#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine($"TerminateTplDataFlowAsync() - конец");
#endif // DEBUG_TPL_ERRORREPORTER
            }

        #endregion Инициализация/Завершение

        #region Завершители

        /// <summary>
        /// Завершение работы буфера
        /// </summary>
        /// <param name="task">Задача завершения</param>
        private void TplDataFlow_DataBufferBlock_Completion(Task task)
            {
#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine($"TplDataFlow_DataBufferBlock_Completion()");
#endif // DEBUG_TPL_ERRORREPORTER
            }

        /// <summary>
        /// Завершение работы актора
        /// </summary>
        /// <param name="task">Задача завершения</param>
        private void TplDataFlow_ActionBlock_Completion(Task task)
            {
#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine($"TplDataFlow_ActionBlock_Completion()");
#endif // DEBUG_TPL_ERRORREPORTER
            }

        #endregion Завершители

        #region добавление сообщений

        /// <summary>
        /// Добавить данные в очередь на обработку
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task TplDataFlowAddDataAsync(ViewPortItem data)
            {
            if (data == null)
                {
                throw new ArgumentNullException(nameof(data), "data не может быть null");
                }
            await _tplDataFlowDataBufferBlock.SendAsync(data);

            Interlocked.Increment(ref _сurrentExecutionStatistics.TplAddedMessages);
            }

        #endregion добавление сообщений

        #region Перегружаемые методы IActorEventsHandler

        /// <summary>
        /// События объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        protected override async void InternalActor_Event(object sender, ActorEventArgs e)
            {
            var actor = sender as ActorBase;
            var viewPortItem = new ViewPortItem(actor, e);
            await TplDataFlowAddDataAsync(viewPortItem);
            }

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        protected override async void InternalActor_StateChangedEvent(object sender, ActorStateChangedEventArgs e)
            {
            var actor = sender as ActorBase;
            var viewPortItem = new ViewPortItem(actor, e);
            await TplDataFlowAddDataAsync(viewPortItem);
            }

        #endregion Перегружаемые методы IActorEventsHandler

        #region Обработка сообщений

        /// <summary>
        /// Обработать сообщение
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        private void TplDataFlowProcessAction(ViewPortItem viewPortItem)
            {
            if (viewPortItem == null)
                {
                return;
                }

            Interlocked.Increment(ref _сurrentExecutionStatistics.TplProcessedMessages);

#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine("TplDataFlowProcessAction()");
#endif // DEBUG_TPL_ERRORREPORTER

            #region Обработка

            if (viewPortItem.ActorEventArgs is ActorStateChangedEventArgs stateEvent)
                {
                ProcessAsActorStateChangedEventArgs(viewPortItem);
                }
            else
            if (viewPortItem.ActorEventArgs is ActorEventArgs actorEvent)
                {
                ProcessAsActorEventArgs(viewPortItem);
                }
            else
                {
                throw new Exception("Непонятно как обрабатывать");
                }

            viewPortItem.Dispose();

            #endregion Обработка
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
        public async void RaiseDebug(string debugText)
            {
            if (NoOutMessages)
                {
                return;
                }

            var ea = new ActorActionEventArgs(debugText, ActorActionEventType.Debug);
            var viewPortItem = new ViewPortItem(emptyActor, ea);
            await TplDataFlowAddDataAsync(viewPortItem);
            }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        public async void RaiseMessage(string messageText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var ea = new ActorActionEventArgs(messageText, ActorActionEventType.Neutral);
            var viewPortItem = new ViewPortItem(emptyActor, ea);
            await TplDataFlowAddDataAsync(viewPortItem);
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="warningText">Текст сообщения</param>
        public async void RaiseWarning(string warningText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var ea = new ActorActionEventArgs(warningText, ActorActionEventType.Warning);
            var viewPortItem = new ViewPortItem(emptyActor, ea);
            await TplDataFlowAddDataAsync(viewPortItem);
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="errorText">Текст сообщения об ошибке</param>
        public async void RaiseError(string errorText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var ea = new ActorActionEventArgs(errorText, ActorActionEventType.Error);
            var viewPortItem = new ViewPortItem(emptyActor, ea);
            await TplDataFlowAddDataAsync(viewPortItem);
            }

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        public async void RaiseException(Exception exception)
            {
            if (NoOutMessages)
                {
                return;
                }
            var ea = new ActorExceptionEventArgs(exception);
            var viewPortItem = new ViewPortItem(emptyActor, ea);
            await TplDataFlowAddDataAsync(viewPortItem);
            }

        #endregion Реализация интерфейса IMessageChannel
        }
    }