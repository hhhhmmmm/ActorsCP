using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActorsCP.Actors.Events;
using ActorsCP.Helpers;
using ActorsCP.Options;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Базовый класс
    /// </summary>
    [DebuggerDisplay("ABN_{ABN}, Name = {Name}, State = {State}")]
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

        /// <summary>
        /// Завершенная задача
        /// </summary>
        protected static Task CompletedTask
            {
            get
                {
                return Task.CompletedTask;
                }
            }

        #endregion Константы для задач

        #region Глобальные внутренние объекты

        /// <summary>
        /// Генератор последовательных номеров объектов
        /// </summary>
        private volatile static int s_ABN_global = 0;

        #endregion Глобальные внутренние объекты

        #region Внутренние объекты

        /// <summary>
        /// Уникальный последовательный номер объекта
        /// </summary>
        private readonly int _ABN = 0;

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
        protected ActorBase()
            {
            _ABN = Interlocked.Increment(ref s_ABN_global); // последовательный номер объекта

            Name = "Объект_" + ABN; // остальные - медленно
            // Name = $"Объект {N} (ActorUid = {ActorUid})"; // // SetName($"Объект {N} (ActorUid = {ActorUid})");
            // Name = $"Объект {N}"; // // SetName($"Объект {N} (ActorUid = {ActorUid})");
            SetPreDisposeHandler(PreDisposeHandler);
            SetRunOnlyOnce(true);

            #region Многословность актора

            var go = GlobalActorOptions.GetInstance();
            if (go.GetInt(ActorKeywords.ActorVerbosity, out int flagVerbosity))
                {
                Verbosity = (ActorVerbosity)flagVerbosity;
                }

            #endregion Многословность актора
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название объекта</param>
        protected ActorBase(string name) : this(name, null)
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parentActor">Родительский объект</param>
        protected ActorBase(ActorBase parentActor) : this(null, parentActor)
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название объекта</param>
        /// <param name="parentActor">Родительский объект</param>
        protected ActorBase(string name, ActorBase parentActor) : this()
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
        public int ABN
            {
            get
                {
                return _ABN;
                }
            }

        #region Parent

        /// <summary>
        /// Родительский объект
        /// </summary>
        public ActorBase Parent
            {
            get
                {
                return _parentActor;
                }
            set
                {
                SetParent(value);
                }
            }

        #endregion Parent

        #region ActorUid

        /// <summary>
        /// Уникальный идентификатор объекта
        /// </summary>
        private Guid _actorUid = Guid.NewGuid();

        /// <summary>
        /// Уникальный идентификатор объекта
        /// </summary>
        public Guid ActorUid
            {
            get
                {
                return _actorUid;
                }
            set
                {
                SetActorUid(value);
                }
            }

        #endregion ActorUid

        #region Название объекта

        /// <summary>
        /// Название объекта
        /// </summary>
        private string _name;

        /// <summary>
        /// Название объекта
        /// </summary>
        public string Name
            {
            get
                {
                return _name;
                }
            set
                {
                SetName(value);
                }
            }

        #endregion Название объекта

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

        #region RunOnlyOnce

        /// <summary>
        /// Разрешен запуск RunAsync() только один раз
        /// </summary>
        private bool _runOnlyOnce;

        /// <summary>
        /// Разрешен запуск RunAsync() только один раз
        /// </summary>
        public bool RunOnlyOnce
            {
            get
                {
                return _runOnlyOnce;
                }
            set
                {
                SetRunOnlyOnce(value);
                }
            }

        #endregion RunOnlyOnce

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

        #region Verbosity

        /// <summary>
        /// Многословность объекта - о каких событиях он будет сообщать
        /// </summary>
        private ActorVerbosity _verbosity = ActorVerbosity.Starting | ActorVerbosity.Running | ActorVerbosity.Stopped;

        /// <summary>
        /// Многословность объекта - о каких событиях он будет сообщать
        /// </summary>
        public ActorVerbosity Verbosity
            {
            get
                {
                return _verbosity;
                }
            set
                {
                SetVerbosity(value);
                }
            }

        #endregion Verbosity

        // ActorVerbosity.Running | ActorVerbosity.Starting | ActorVerbosity.Started | ActorVerbosity.Stopping | ActorVerbosity.Stopped;

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
        private void PreDisposeHandler()
            {
            if (State != ActorState.Terminated)
                {
                TerminateAsync().Wait(); // await работает медленно
                }
            }

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            if (State != ActorState.Terminated)
                {
                TerminateAsync().Wait(); // await работает медленно
                }

            // внешние ссылки не очищаем так как не мы их туда добавляли
            // _externalObjects?.Clear();
            // _externalObjects = null;

            if (_cancellationTokenSource != null)
                {
                _IsCanceled = _cancellationTokenSource.IsCancellationRequested;
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                }
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
        protected async void SetActorState(ActorState newState)
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
                    RaiseActorStateChanged(ActorStates.ActorStateChangedPending);
                    break;
                    }
                case ActorState.Started:
                    {
                    RaiseActorStateChanged(ActorStates.ActorStateChangedStarted);
                    break;
                    }
                case ActorState.Stopped:
                    {
                    RaiseActorStateChanged(ActorStates.ActorStateChangedStopped);
                    if (RunOnlyOnce) // если объект запускается только один раз, то нужно его сразу завершить
                        {
                        await TerminateAsync().ConfigureAwait(false);
                        }
                    break;
                    }
                case ActorState.Running:
                    {
                    RaiseActorStateChanged(ActorStates.ActorStateChangedRunning);
                    break;
                    }
                case ActorState.Terminated:
                    {
                    RaiseActorStateChanged(ActorStates.ActorStateChangedTerminated);
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
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} не может быть null");
                }

            if (this.Name != null)
                {
                if (!this.Name.Equals(name))
                    {
                    _name = name;
                    }
                }
            else
                {
                _name = name;
                }
            }

        #endregion Защищенные методы

        #region Методы

        /// <summary>
        /// Установить уникальный идентификатор объекта
        /// </summary>
        /// <param name="actorUid">Уникальный идентификатор объекта</param>
        public void SetActorUid(Guid actorUid)
            {
            _actorUid = actorUid;
            }

        /// <summary>
        /// Установить многословность объекта - о каких событиях он будет сообщать
        /// </summary>
        /// <param name="verbosity">Многословность объекта</param>
        public void SetVerbosity(ActorVerbosity verbosity)
            {
            _verbosity = verbosity;
            }

        /// <summary>
        /// Установить флаг разрешения запуска только один раз
        /// </summary>
        /// <param name="runOnlyOnce"></param>
        public void SetRunOnlyOnce(bool runOnlyOnce = true)
            {
            _runOnlyOnce = runOnlyOnce;
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
            return ABN;
            }

        #endregion Методы
        } // end class Actor
    } // end namespace ActorsCP.Actors