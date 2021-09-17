using System;
using System.Text;
using System.Threading;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Базовый класс вьюпорта со всемми интерфейсами
    /// </summary>
    public class ActorViewPortBase : IActorViewPort, IActorBindViewPortHandler, IActorEventsHandler
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parameter"></param>
        public ActorViewPortBase()
            {
            }

        #endregion Конструкторы

        #region Приватные мемберы

        /// <summary>
        /// Статистика выполнения
        /// </summary>
        private ExecutionStatistics _сurrentExecutionStatistics;

        #endregion Приватные мемберы

        #region Свойства

        /// <summary>
        /// Статистика выполнения
        /// </summary>
        public ExecutionStatistics СurrentExecutionStatistics
            {
            get
                {
                return _сurrentExecutionStatistics;
                }
            }

        /// <summary>
        /// Не выводить сообщения
        /// </summary>
        public bool NoOutMessages
            {
            get;
            set;
            } = false;

        #endregion Свойства

        #region Реализация интерфейса IActorBindViewPortHandler

        /// <summary>
        /// Вызывается, когда объект подписан на события
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        public void Actor_ViewPortBound(ActorBase actor)
            {
            Interlocked.Increment(ref _сurrentExecutionStatistics.TotalBoundObjects);
            Interlocked.Increment(ref _сurrentExecutionStatistics.BoundObjects);
            InternalActor_ViewPortBound(actor);
            }

        /// <summary>
        /// Вызывается, когда объект отписан от событий
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        public void Actor_ViewPortUnbound(ActorBase actor)
            {
            Interlocked.Increment(ref _сurrentExecutionStatistics.TotalUnboundObjects);
            Interlocked.Decrement(ref _сurrentExecutionStatistics.BoundObjects);
            InternalActor_ViewPortUnbound(actor);
            }

        #endregion Реализация интерфейса IActorBindViewPortHandler

        #region Перегружаемые методы IActorBindViewPortHandler

        /// <summary>
        /// Вызывается, когда объект подписан на события
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        protected virtual void InternalActor_ViewPortBound(ActorBase actor)
            {
            }

        /// <summary>
        /// Вызывается, когда объект отписан от событий
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        protected virtual void InternalActor_ViewPortUnbound(ActorBase actor)
            {
            }

        #endregion Перегружаемые методы IActorBindViewPortHandler

        #region Реализация интерфейса IActorEventsHandler

        /// <summary>
        /// События объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        public void Actor_Event(object sender, ActorEventArgs e)
            {
            if (e is ActorExceptionEventArgs) // исключение
                {
                Interlocked.Increment(ref _сurrentExecutionStatistics.Exceptions);
                }
            if (e is ActorActionEventArgs a && a.ActionEventType == ActorActionEventType.Error)
                {
                Interlocked.Increment(ref _сurrentExecutionStatistics.Errors);
                }

            if (NoOutMessages)
                {
                return;
                }
            InternalActor_Event(sender, e);
            }

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        public void Actor_StateChangedEvent(object sender, ActorStateChangedEventArgs e)
            {
            if (e is ActorSetCountChangedEventArgs) // изменилось состояние запущенной очереди или толпы
                {
                return;
                }

            if (e.State == ActorState.Started)
                {
                Interlocked.Increment(ref _сurrentExecutionStatistics.StartedObjects);
                Interlocked.Increment(ref _сurrentExecutionStatistics.RunningObjects);
                }
            if (e.State == ActorState.Stopped)
                {
                Interlocked.Increment(ref _сurrentExecutionStatistics.StoppedObjects);
                Interlocked.Decrement(ref _сurrentExecutionStatistics.RunningObjects);
                }

            if (e.State == ActorState.Terminated)
                {
                Interlocked.Increment(ref _сurrentExecutionStatistics.TerminatedObjects);
                }

            InternalActor_StateChangedEvent(sender, e);
            }

        #endregion Реализация интерфейса IActorEventsHandler

        #region Перегружаемые методы IActorEventsHandler

        /// <summary>
        /// События объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        protected virtual void InternalActor_Event(object sender, ActorEventArgs e)
            {
            }

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        protected virtual void InternalActor_StateChangedEvent(object sender, ActorStateChangedEventArgs e)
            {
            }

        #endregion Перегружаемые методы IActorEventsHandler

        /// <summary>
        /// Проверить корректность собранных значений счетчиков
        /// </summary>
        public void ValidateStatistics()
            {
            СurrentExecutionStatistics.ValidateStatistics();
            }
        } // end class ViewPortBase
    } // end namespace ActorsCP.ViewPorts