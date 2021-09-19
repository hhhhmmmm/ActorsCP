using System;
using System.Text;
using System.Threading;
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
        protected static Task<bool> CompletedTaskBoolTrue
            {
            get
                {
                return TasksHelper.CompletedTaskBoolTrue;
                }
            }

        /// <summary>
        /// Неуспешно завершенная задача
        /// </summary>
        protected static Task<bool> CompletedTaskBoolFalse
            {
            get
                {
                return TasksHelper.CompletedTaskBoolFalse;
                }
            }

        #endregion Константы для задач

        #region Глобальные внутренние объекты

        /// <summary>
        /// Генератор последовательных номеров объектов
        /// </summary>
        private static int s_N_global = 0;

        #endregion Глобальные внутренние объекты

        #region Внутренние объекты

        /// <summary>
        /// Уникальный последовательный номер объекта
        /// </summary>
        private int _N = 0;

        /// <summary>
        /// Родительский объект
        /// </summary>
        private ActorBase _parentActor;

        /// <summary>
        /// Глобальный объект для синхронизации доступа
        /// </summary>
        protected readonly object Locker = new object();

        /// <summary>
        /// Время выполнения Run()
        /// </summary>
        private ActorTime _executionTime = default;

        #endregion Внутренние объекты

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorBase()
            {
            _N = Interlocked.Increment(ref s_N_global); // последовательный номер объекта

            SetName($"Объект {N} (ActorUid = {ActorUid})");
            SetPreDisposeHandler(PreDisposeHandler);
            SetRunOnlyOnce(true);
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название объекта</param>
        public ActorBase(string name) : this(name, null)
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parentActor">Родительский объект</param>
        public ActorBase(ActorBase parentActor) : this(null, parentActor)
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название объекта</param>
        /// <param name="parentActor">Родительский объект</param>
        public ActorBase(string name, ActorBase parentActor) : this()
            {
            if (!string.IsNullOrEmpty(name))
                {
                SetName(name);
                }
            if (parentActor != null)
                {
                SetParent(parentActor);
                }
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Уникальный последовательный номер объекта
        /// </summary>
        public int N
            {
            get
                {
                return _N;
                }
            }

        /// <summary>
        /// Родительский объект
        /// </summary>
        public ActorBase Parent
            {
            get
                {
                return _parentActor;
                }
            }

        /// <summary>
        /// Уникальный идентификатор объекта
        /// </summary>
        public Guid ActorUid
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
        /// Состояние объекта
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
                return _executionTime;
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

        /// <summary>
        /// В процессе работы возникли ошибки
        /// </summary>
        public bool AnErrorOccurred
            {
            get;
            private set;
            }

        #endregion Свойства

        #region Блок завершения

        /// <summary>
        /// Был вызван InternalRunCleanupBeforeTerminationAsync()
        /// </summary>
        private bool _internalRunCleanupBeforeTermination;

        /// <summary>
        /// Результат выполнения RunCleanupBeforeTerminationAsync()
        /// </summary>
        private bool _runCleanupBeforeTerminationAsyncResult;

        /// <summary>
        /// Вызывает InternalRunCleanupBeforeTermination() один раз
        /// </summary>
        /// <param name="fromDispose">Вызов из Dispose()</param>
        /// <returns></returns>
        private async Task<bool> RunCleanupBeforeTerminationAsync(bool fromDispose)
            {
            if (_internalRunCleanupBeforeTermination)
                {
                return _runCleanupBeforeTerminationAsyncResult;
                }
            else
                {
                _runCleanupBeforeTerminationAsyncResult = await InternalRunCleanupBeforeTerminationAsync(fromDispose).ConfigureAwait(false);
                _internalRunCleanupBeforeTermination = true;
                return _runCleanupBeforeTerminationAsyncResult;
                }
            }

        #endregion Блок завершения

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Метод вызывается перед началом Dispose
        /// </summary>
        private async void PreDisposeHandler()
            {
            if (State != ActorState.Terminated)
                {
                await TerminateAsync().ConfigureAwait(false);
                }
            }

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override async void DisposeManagedResources()
            {
            if (State != ActorState.Terminated)
                {
                await TerminateAsync().ConfigureAwait(false);
                }

            _externalObjects?.Clear();
            _externalObjects = null;

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _parentActor = null;
            _iMessageChannel = null;
            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable

        #region Защищенные методы

        /// <summary>
        /// Установить флаг AnErrorOccurred
        /// </summary>
        protected void SetAnErrorOccurred()
            {
            AnErrorOccurred = true;
            }

        /// <summary>
        /// Установить новое состояние объекта
        /// </summary>
        /// <param name="newState">новое состояние объекта</param>
        protected void SetActorState(ActorState newState)
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
                    RaiseActorStateChanged(ActorStates.Pending);
                    break;
                    }
                case ActorState.Started:
                    {
                    RaiseActorEvent(ActorStates.Started);
                    RaiseActorStateChanged(ActorStates.Started);
                    break;
                    }
                case ActorState.Stopped:
                    {
                    RaiseActorEvent(ActorStates.Stopped);
                    RaiseActorStateChanged(ActorStates.Stopped);
                    break;
                    }
                case ActorState.Running:
                    {
                    RaiseActorEvent(ActorStates.Running);
                    RaiseActorStateChanged(ActorStates.Running);
                    break;
                    }
                case ActorState.Terminated:
                    {
                    RaiseActorEvent(ActorStates.Terminated);
                    RaiseActorStateChanged(ActorStates.Terminated);
                    UnbindAllViewPorts();
                    ClearViewPortHelper(); // В SetActorState(Terminated); // отвязываем все порты так как перешли в состояние Terminated и больше сообщений посылать не будем
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
        public void SetName(string name)
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

        #region Методы

        /// <summary>
        /// Установить флаг разрешения запуска только один раз
        /// </summary>
        /// <param name="runOnlyOnce"></param>
        public void SetRunOnlyOnce(bool runOnlyOnce = true)
            {
            RunOnlyOnce = runOnlyOnce;
            }

        /// <summary>
        /// Установить родительский объект
        /// </summary>
        /// <param name="parentActor">Родительский объект</param>
        public virtual void SetParent(ActorBase parentActor)
            {
            if (_parentActor == parentActor)
                {
                return;
                }

            _parentActor = parentActor;
            if (_parentActor != null)
                {
                SetIMessageChannel(_parentActor.MessageChannel);
                // UnsubscribeFromCancelationEvents(_parentActor);
                }

            //CreateCancellationTokenSource(parentActor);
            //SubscribeToCancelationEvents(parentActor);
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

        /// <summary>
        /// Хэш
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            {
            return N;
            }

        #endregion Методы
        } // end class Actor
    } // end namespace ActorsCP.Actors