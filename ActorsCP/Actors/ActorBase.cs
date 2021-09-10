using System;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Actors.Events;
using ActorsCP.Helpers;

namespace ActorsCP.Actors
    {
    /// <summary>
    ///
    /// </summary>
    public abstract partial class ActorBase : DisposableImplementation<ActorBase>, IMessageChannel
        {
        #region Константы для задач

        /// <summary>
        /// Успешно завершенная задача
        /// </summary>
        protected Task<bool> CompletedTaskBoolTrue
            {
            get
                {
                return TasksHelper.CompletedTaskBoolTrue;
                }
            }

        /// <summary>
        /// Неуспешно завершенная задача
        /// </summary>
        protected Task<bool> CompletedTaskBoolFalse
            {
            get
                {
                return TasksHelper.CompletedTaskBoolFalse;
                }
            }

        #endregion Константы для задач

        #region Внутренние объекты

        /// <summary>
        /// Родительский объект
        /// </summary>
        private ActorBase m_ParentActor;

        /// <summary>
        /// Глобальный объект для синхронизации доступа
        /// </summary>
        protected readonly object Locker = new object();

        /// <summary>
        /// Время выполнения Run()
        /// </summary>
        private ActorTime m_ExecutionTime = default(ActorTime);

        /// <summary>
        /// Канал сообщений
        /// </summary>
        private IMessageChannel m_IMessageChannel;

        #endregion Внутренние объекты

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorBase()
            {
            SetName($"Безымянный объект ActorUID = {ActorUID}");
            SetPreDisposeHandler(PreDisposeHandler);
            InitLogger();
            }

        /// <summary>Конструктор</summary>
        /// <param name="name">Название объекта</param>
        protected ActorBase(string name)
            {
            SetName(name);
            SetPreDisposeHandler(PreDisposeHandler);
            InitLogger();
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Родительский объект
        /// </summary>
        public ActorBase Parent
            {
            get
                {
                return m_ParentActor;
                }
            }

        /// <summary>
        /// Уникальный идентификатор объекта
        /// </summary>
        public Guid ActorUID
            {
            get;
            protected set;
            } = Guid.NewGuid();

        /// <summary>
        /// Название объекта
        /// </summary>
        public string Name
            {
            get;
            private set;
            }

        /// <summary>
        /// Состояние актора
        /// </summary>
        public ActorState State
            {
            get;
            private set;
            } = ActorState.Pending;

        /// <summary>
        /// Время выполнения Run()
        /// </summary>
        public ActorTime ExecutionTime
            {
            get
                {
                return m_ExecutionTime;
                }
            }

        /// <summary>
        /// Запущено
        /// </summary>
        public bool IsStarted
            {
            get
                {
                switch (State)
                    {
                    case ActorState.Started:
                    case ActorState.Running:
                        {
                        return true;
                        }
                    default:
                        {
                        return false;
                        }
                    }
                }
            }

        /// <summary>
        /// Остановлено
        /// </summary>
        public bool IsStopped
            {
            get
                {
                return !IsStarted;
                }
            }

        /// <summary>
        /// Выполняется
        /// </summary>
        public bool IsRunning
            {
            get
                {
                return State == ActorState.Running;
                }
            }

        /// <summary>
        /// В ожидании
        /// </summary>
        public bool IsPending
            {
            get
                {
                return State == ActorState.Pending;
                }
            }

        /// <summary>
        /// Завершен
        /// </summary>
        public bool IsTerminated
            {
            get
                {
                return State == ActorState.Terminated;
                }
            }

        #endregion Свойства

        #region Блок завершения

        /// <summary>
        /// Был вызван InternalRunCleanupBeforeTerminationAsync()
        /// </summary>
        private bool m_InternalRunCleanupBeforeTermination;

        /// <summary>
        /// Результат выполнения RunCleanupBeforeTerminationAsync()
        /// </summary>
        private bool m_RunCleanupBeforeTerminationAsyncResult;

        /// <summary>
        /// Вызывает RunCleanupBeforeTermination() один раз
        /// </summary>
        private async Task<bool> InternalRunCleanupBeforeTerminationAsync()
            {
            if (m_InternalRunCleanupBeforeTermination)
                {
                return m_RunCleanupBeforeTerminationAsyncResult;
                }
            else
                {
                m_RunCleanupBeforeTerminationAsyncResult = await RunCleanupBeforeTerminationAsync();
                m_InternalRunCleanupBeforeTermination = true;
                return m_RunCleanupBeforeTerminationAsyncResult;
                }
            }

        /// <summary>
        /// Метод вызывается до отправки сигнала OnActorTerminated и предназначен для очистки объекта
        /// от хранимых в нем временных данных. Также вызывается из Dispose()
        /// </summary>
        protected virtual Task<bool> RunCleanupBeforeTerminationAsync()
            {
            return CompletedTaskBoolTrue;
            }

        #endregion Блок завершения

        #region Реализация интерфейса IDisposable

        /// <summary>Метод вызывается перед началом Dispose</summary>
        private async void PreDisposeHandler()
            {
            await TerminateAsync();
            }

        /// <summary>Освободить управляемые ресурсы</summary>
        protected override void DisposeManagedResources()
            {
            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable

        #region Защищенные методы

        /// <summary>
        /// Установить новое состояние объекта
        /// </summary>
        /// <param name="newState">новое состояние актора</param>
        private void SetActorState(ActorState newState)
            {
            if (State == newState)
                {
                return;
                }

            if (State == ActorState.Terminated)
                {
                throw new InvalidOperationException($"Актор {Name} находится в состоянии Terminated, изменение состояния невозможно");
                }

            State = newState;

            switch (State)
                {
                case ActorState.Pending:
                    {
                    RaiseActorEvent(ActorStates.Pending);
                    break;
                    }
                case ActorState.Started:
                    {
                    RaiseActorEvent(ActorStates.Started);
                    break;
                    }
                case ActorState.Stopped:
                    {
                    RaiseActorEvent(ActorStates.Stopped);
                    break;
                    }
                case ActorState.Running:
                    {
                    RaiseActorEvent(ActorStates.Running);
                    break;
                    }
                case ActorState.Terminated:
                    {
                    RaiseActorEvent(ActorStates.Terminated);
                    break;
                    }
                default:
                    {
                    throw new InvalidOperationException($"Неизвестное событие {State}");
                    }
                }
            }

        /// <summary>
        /// Установить название объекта
        /// </summary>
        /// <param name="name">Название объекта</param>
        protected void SetName(string name)
            {
            if (string.IsNullOrEmpty(name))
                {
                throw new ArgumentNullException(name, "Имя объекта нужно указать");
                }

            if (this.Name != null)
                {
                if (!this.Name.Equals(name))
                    {
                    this.Name = name;
                    }
                }
            else
                {
                this.Name = name;
                }
            }

        #endregion Защищенные методы

        #region События объекта

        /// <summary>
        /// Выкинуть событие
        /// </summary>
        /// <param name="actorEventArgs">Событие</param>
        protected void RaiseActorEvent(ActorEventArgs actorEventArgs)
            {
            m_Events?.Invoke(this, actorEventArgs);
            }

        /// <summary>
        /// События объекта
        /// </summary>
        private event EventHandler<ActorEventArgs> m_Events;

        /// <summary>
        /// События объекта
        /// </summary>
        public event EventHandler<ActorEventArgs> Events
            {
            add
                {
                m_Events += value;
                }
            remove
                {
                m_Events -= value;
                }
            }

        #endregion События объекта

        #region Методы

        /// <summary>
        /// Частичная реализация - инициализация логгера
        /// </summary>
        private void InitLogger()
            {
            InternalInitLogger();
            }

        /// <summary>
        /// Установить родительский объект
        /// </summary>
        /// <param name="parentActor">Родительский объект</param>
        public void SetParent(ActorBase parentActor)
            {
            if (m_ParentActor == parentActor)
                {
                return;
                }

            m_ParentActor = parentActor;
            if (m_ParentActor != null)
                {
                SetIMessageChannel(m_ParentActor);
                // UnsubscribeFromCancelationEvents(m_ParentActor);
                }

            //CreateCancellationTokenSource(parentActor);
            //SubscribeToCancelationEvents(parentActor);
            }

        /// <summary>
        /// Установить указатель на канал сообщений
        /// </summary>
        /// <param name="iMessageChannel">Канал сообщений</param>
        public void SetIMessageChannel(IMessageChannel iMessageChannel)
            {
            if (m_IMessageChannel == iMessageChannel)
                {
                return;
                }
            if (m_IMessageChannel == this)
                {
                return;
                }
            m_IMessageChannel = iMessageChannel;
            }

        #endregion Методы

        #region IMessageChannel

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        public void RaiseDebug(string debugText)
            {
            var a = new ActorActionEventArgs(debugText, ActorActionEventType.Debug);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseDebug(debugText);
            }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        public void RaiseMessage(string messageText)
            {
            var a = new ActorActionEventArgs(messageText, ActorActionEventType.Neutral);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseMessage(messageText);
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="warningText">Текст сообщения c предупреждением</param>
        public void RaiseWarning(string warningText)
            {
            var a = new ActorActionEventArgs(warningText, ActorActionEventType.Warning);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseWarning(warningText);
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="errorText">Текст сообщения об ошибке</param>
        public void RaiseError(string errorText)
            {
            var a = new ActorActionEventArgs(errorText, ActorActionEventType.Error);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseError(errorText);
            }

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void RaiseException(Exception exception)
            {
            var a = new ActorExceptionEventArgs(exception);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseException(exception);
            }

        #endregion IMessageChannel

        #region Набор методов для переопределения

        /// <summary>
        /// Частичная реализация - инициализация логгера
        /// </summary>
        protected virtual void InternalInitLogger()
            {
            }

        /// <summary>
        /// Запуск
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected virtual Task<bool> InternalStartAsync()
            {
            return CompletedTaskBoolTrue;
            }

        /// <summary>
        /// Остановка
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected virtual Task<bool> InternalStopAsync()
            {
            return CompletedTaskBoolTrue;
            }

        /// <summary>
        /// Перегружаемая функция для выполнения некоторых действий
        /// </summary>
        /// <returns>true если объект все выполнил успешно</returns>
        protected virtual Task<bool> InternalRunAsync()
            {
            return CompletedTaskBoolTrue;
            }

        #endregion Набор методов для переопределения

        /// <summary>
        /// Полная остановка
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TerminateAsync()
            {
            if (State == ActorState.Terminated)
                {
                return true;
                }
            try
                {
                var bres = await InternalRunCleanupBeforeTerminationAsync();
                return bres;
                }
            catch (Exception ex)
                {
                RaiseException(ex);
                return false;
                }
            finally
                {
                SetActorState(ActorState.Terminated);
                }
            }

        /// <summary>
        /// Запуск
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StartAsync()
            {
            try
                {
                if (IsStarted)
                    {
                    return true;
                    }
                RaiseMessage($"Запуск {Name}...");
                var bres = await InternalStartAsync();
                if (bres)
                    {
                    SetActorState(ActorState.Started);
                    RaiseMessage($"{Name} успешно запущен");
                    return true;
                    }
                RaiseMessage($"Ошибка запуска {Name}");
                return false;
                }
            catch (Exception e)
                {
                RaiseException(e);
                await TerminateAsync();
                return false;
                }
            }

        /// <summary>
        /// Остановка
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopAsync()
            {
            try
                {
                if (!IsStarted && !IsRunning)
                    {
                    return true;
                    }
                RaiseMessage($"Остановка {Name}...");
                var bres = await InternalStopAsync();
                if (bres)
                    {
                    RaiseMessage($"{Name} остановлен");
                    SetActorState(ActorState.Stopped);
                    return true;
                    }
                return false;
                }
            catch (Exception e)
                {
                RaiseException(e);
                await TerminateAsync();
                return false;
                }
            }

        /// <summary>
        /// Выполнить некоторую работу. Внутри себя метод Run() вызывает перегружаемый метод InternalRunAsync()
        /// </summary>
        /// <returns>true если работа успешно завершена</returns>
        public async Task<bool> RunAsync()
            {
            bool bres = false;
            try
                {
                if (IsTerminated)
                    {
                    RaiseError("Попытка запуска завершенного объекта");
                    return false;
                    }

                if (IsRunning)
                    {
                    RaiseError("Попытка запуска выполняющегося объекта");
                    return false;
                    }

                #region Выполнение

                using (var gt = new ActorDisposableTime($"выполнения '{Name}'", RaiseMessage))
                    {
                    if (!IsStarted)
                        {
                        bres = await StartAsync();
                        if (!bres)
                            {
                            return false;
                            }
                        }

                    m_ExecutionTime.SetStartDate();
                    SetActorState(ActorState.Running);
                    var runtask = InternalRunAsync();
                    await (runtask);
                    m_ExecutionTime.SetEndDate();

                    if (IsStarted || IsRunning)
                        {
                        bres = await StopAsync();
                        if (!bres)
                            {
                            await TerminateAsync();
                            return false;
                            }
                        }

                    bres = await TerminateAsync();
                    if (!bres)
                        {
                        return false;
                        }
                    return runtask.Result;
                    }

                #endregion Выполнение
                } // end try
            catch (AggregateException ae)
                {
                m_ExecutionTime.SetEndDate();

                foreach (Exception e in ae.InnerExceptions)
                    {
                    RaiseException(e);
                    }
                await TerminateAsync();
                return false;
                }
            catch (Exception e)
                {
                m_ExecutionTime.SetEndDate();

                RaiseException(e);
                await TerminateAsync();
                return false;
                } // end catch
            }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            {
            if (!string.IsNullOrEmpty(Name))
                {
                return Name;
                }
            return "Безымянный объект";
            }
        } // end class Actor
    } // end namespace ActorsCP.Actors