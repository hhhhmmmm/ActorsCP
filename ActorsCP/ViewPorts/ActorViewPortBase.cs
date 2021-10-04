using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;
using ActorsCP.Helpers;
using ActorsCP.Options;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Базовый класс вьюпорта со всеми интерфейсами
    /// </summary>
    public class ActorViewPortBase : DisposableImplementation<ActorViewPortBase>,
        IActorViewPort,
        IActorBindViewPortHandler,
        IActorEventsHandler
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorViewPortBase()
            {
            }

        #endregion Конструкторы

        #region Приватные мемберы

        /// <summary>
        /// Отладка сообщений вьюпорта об изменении состояния набора
        /// </summary>
        protected bool _ViewPort_DebugStateChangedEvent;

        /// <summary>
        /// Статистика выполнения
        /// </summary>
        protected ExecutionStatistics _сurrentExecutionStatistics;

        /// <summary>
        /// Локер
        /// </summary>
        protected static readonly object Locker = new object();

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

        /// <summary>
        /// Вьюпорт инициализирован
        /// </summary>
        public bool IsInitialized
            {
            get;
            protected set;
            }

        /// <summary>
        /// Вьюпорт завершен
        /// </summary>
        public bool IsTerminated
            {
            get;
            protected set;
            }

        #endregion Свойства

        #region Реализация интерфейса IActorBindViewPortHandler

        /// <summary>
        /// Вызывается, когда объект подписан на события или отписан от них
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        /// <param name="actorViewPortBoundEventArgs">Событие - объект привязан или отвязан</param>
        public void Actor_ViewPortBoundUnbound(ActorBase actor, ActorViewPortBoundEventArgs actorViewPortBoundEventArgs)
            {
            if (actorViewPortBoundEventArgs.Bound)
                {
                Interlocked.Increment(ref _сurrentExecutionStatistics.TotalBoundObjects);
                Interlocked.Increment(ref _сurrentExecutionStatistics.BoundObjects);
                }
            else
                {
                Interlocked.Increment(ref _сurrentExecutionStatistics.TotalUnboundObjects);
                Interlocked.Decrement(ref _сurrentExecutionStatistics.BoundObjects);
                }
            InternalActor_ViewPortBoundUnbound(actor, actorViewPortBoundEventArgs);
            }

        #endregion Реализация интерфейса IActorBindViewPortHandler

        #region Перегружаемые методы IActorBindViewPortHandler

        /// <summary>
        /// Вызывается, когда объект подписан на события или отписан от них
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        /// <param name="actorViewPortBoundEventArgs">Событие - объект привязан или отвязан</param>
        protected virtual void InternalActor_ViewPortBoundUnbound(ActorBase actor, ActorViewPortBoundEventArgs actorViewPortBoundEventArgs)
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
        /// Вывести отладочное сообщение во вьюпорт
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        private void RaiseDebugEvent(string text)
            {
            var a = new ActorActionEventArgs("Отладка вьюпорта: " + text, ActorActionEventType.Debug);
            InternalActor_Event(this, a);
            }

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        public void Actor_StateChangedEvent(object sender, ActorStateChangedEventArgs e)
            {
            if (e is ActorSetCountChangedEventArgs i) // изменилось состояние запущенной очереди или толпы
                {
                Interlocked.Increment(ref _сurrentExecutionStatistics.StateChanged);

                if (_ViewPort_DebugStateChangedEvent) //  Отладка сообщений вьюпорта об изменении состояния набора
                    {
                    var str = $" Actor_StateChangedEvent: W-{i.WaitingCount}, R-{i.RunningCount}, C-{i.CompletedCount}, State={i.State}";
                    RaiseDebugEvent(str);
                    }

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

        #region Публичные методы

        /// <summary>
        /// Увеличить число показанных элементов на экране
        /// </summary>
        public void Increment_СurrentExecutionStatistics_ViewPortDisplayed()
            {
            Interlocked.Increment(ref _сurrentExecutionStatistics.ViewPortDisplayed);
            }

        /// <summary>
        /// Проверить корректность собранных значений счетчиков
        /// </summary>
        public void ValidateStatistics()
            {
            СurrentExecutionStatistics.ValidateStatistics();
            }

        /// <summary>
        /// Инициализация вьюпорта
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        public void Init(string additionalText = null)
            {
            InternalInit(additionalText);
            }

        /// <summary>
        /// Завершение вьюпорта
        /// </summary>
        public void Terminate()
            {
            InternalTerminate();
            }

        /// <summary>
        /// Перенастроить вьюпорт
        /// </summary>
        public void Reconfigure()
            {
            InternalReconfigure();
            }

        /// <summary>
        /// Установить заголовок
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        public void SetTitle(string additionalText = null)
            {
            InternalSetTitle(additionalText);
            }

        #endregion Публичные методы

        #region Перегружаемые методы

        /// <summary>
        /// Установить заголовок
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        public virtual void InternalSetTitle(string additionalText)
            {
            }

        /// <summary>
        /// Инициализация вьюпорта
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        protected virtual void InternalInit(string additionalText)
            {
            IsInitialized = true;
            }

        /// <summary>
        /// Завершение вьюпорта
        /// </summary>
        protected virtual void InternalTerminate()
            {
            IsTerminated = true;
            }

        /// <summary>
        /// Перенастроить вьюпорт
        /// </summary>
        protected virtual void InternalReconfigure()
            {
            var gado = GlobalActorDebugOptions.GetInstance();
            gado.GetBool(ActorDebugKeywords.ViewPort_DebugStateChangedEvent, out _ViewPort_DebugStateChangedEvent);
            }

        #endregion Перегружаемые методы

        /// <summary>
        /// Возвращает версию файла сборки (та, которая указывается в AssemblyFileVersion в файле AssemblyInfo.cs)
        /// </summary>
        /// <returns></returns>
        protected static string GetAssemblyFileVersion()
            {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            return version;
            }
        } // end class ViewPortBase
    } // end namespace ActorsCP.ViewPorts