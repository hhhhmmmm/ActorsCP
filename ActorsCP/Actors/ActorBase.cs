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
            SetRunOnlyOnce(true);
            }

        /// <summary>Конструктор</summary>
        /// <param name="name">Название объекта</param>
        protected ActorBase(string name) : this()
            {
            SetName(name);
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
        /// Разрешен запуск RunAsync() только один раз
        /// </summary>
        public bool RunOnlyOnce
            {
            get;
            private set;
            }

        /// <summary>
        /// RunAsync() уже выполнялась
        /// </summary>
        public bool HasBeenRun
            {
            get;
            private set;
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
        /// Вызывает InternalRunCleanupBeforeTermination() один раз
        /// </summary>
        private async Task<bool> RunCleanupBeforeTerminationAsync()
            {
            if (m_InternalRunCleanupBeforeTermination)
                {
                return m_RunCleanupBeforeTerminationAsyncResult;
                }
            else
                {
                m_RunCleanupBeforeTerminationAsyncResult = await InternalRunCleanupBeforeTerminationAsync();
                m_InternalRunCleanupBeforeTermination = true;
                return m_RunCleanupBeforeTerminationAsyncResult;
                }
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
        /// Внутренняя очистка
        /// </summary>
        private void CleanUp()
            {
            m_ParentActor = null;
            m_IMessageChannel = null;
            }

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
                    CleanUp();
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

        /// <summary>
        /// Частичная реализация - инициализация логгера
        /// </summary>
        private void InitLogger()
            {
            InternalInitLogger();
            }

        #endregion Защищенные методы

        #region Методы

        /// <summary>
        /// Установить флаг разрешения запуска только один раз
        /// </summary>
        /// <param name="runOnlyOnce"></param>
        public void SetRunOnlyOnce(bool runOnlyOnce)
            {
            RunOnlyOnce = runOnlyOnce;
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

        #endregion Методы

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