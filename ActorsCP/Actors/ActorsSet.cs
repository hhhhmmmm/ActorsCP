using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ActorsCP.Actors.Events;
using ActorsCP.Helpers;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Набор объектов для построения очередей и куч
    /// </summary>
    public class ActorsSet : ActorBase
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorsSet()
            {
            SetName("Набор объектов");
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название объекта</param>
        public ActorsSet(string name) : base(name)
            {
            }

        #endregion Конструкторы

        #region Свойства

        ///// <summary>
        ///// Количество объектов ожидающих выполнения
        ///// </summary>
        public int WaitingCount
            {
            get
                {
                return _waitingCount;
                }
            }

        ///// <summary>
        /////  Количество выполняющихся объектов
        ///// </summary>
        public int RunningCount
            {
            get
                {
                return _runningCount;
                }
            }

        ///// <summary>
        /////  Количество завершенных объектов
        ///// </summary>
        public int CompletedCount
            {
            get
                {
                return _completedCount;
                }
            }

        /// <summary>
        /// Общее количество объектов
        /// </summary>
        public int TotalCount
            {
            get
                {
                return _waitingCount + _runningCount + _completedCount;
                }
            }

        #endregion Свойства

        #region Приватные мемберы

        ///// <summary>
        ///// Список объектов ожидающих выполнения
        ///// </summary>
        protected readonly List<ActorBase> _waiting = new List<ActorBase>();

        /// <summary>
        /// Количество объектов ожидающих выполнения
        /// </summary>
        private volatile int _waitingCount = 0;

        /// <summary>
        /// Список выполняющихся объектов
        /// </summary>
        private readonly ConcurrentContainerT<ActorBase> _running = new ConcurrentContainerT<ActorBase>();

        /// <summary>
        /// Количество выполняющихся объектов
        /// </summary>
        private volatile int _runningCount = 0;

        /// <summary>
        /// Список завершенных объектов
        /// </summary>
        private readonly ConcurrentContainerT<ActorBase> _completed = new ConcurrentContainerT<ActorBase>();

        /// <summary>
        /// Количество завершенных объектов
        /// </summary>
        private volatile int _completedCount = 0;

        #endregion Приватные мемберы

        /// <summary>
        /// Проверить состояние набора объектов
        /// </summary>
        private void CheckActorsSetState()
            {
            if (Disposed)
                {
                throw new InvalidOperationException($"Набор объектов {Name} уже Disposed()");
                }
            if (State == ActorState.Terminated)
                {
                throw new InvalidOperationException($"Набор объектов {Name} в состоянии Terminated");
                }
            }

        /// <summary>
        /// Отправить событие о том, что состояние набора изменилось
        /// </summary>
        /// <param name="actor"></param>
        private void RaiseActorsSetChanged(ActorBase actor)
            {
            var ev = new ActorSetCountChangedEventArgs(_waitingCount, _runningCount, _completedCount, State);

#if DEBUG
            Debug.WriteLine($"RaiseActorsSetChanged(): Объект:{ actor.State}, очередь: {ev}");
#endif //
            RaiseActorStateChanged(ev);
            }

        /// <summary>
        /// Метод вызывается до отправки сигнала OnActorTerminated и предназначен для очистки объекта
        /// от хранимых в нем временных данных. Также вызывается из Dispose()
        /// </summary>
        protected override async Task<bool> InternalRunCleanupBeforeTerminationAsync()
            {
            var bres = await base.InternalRunCleanupBeforeTerminationAsync();

            lock (Locker)
                {
                _waiting?.Clear();
                _waitingCount = 0;
                _running?.Clear();
                _runningCount = 0;
                _completed?.Clear();
                _completedCount = 0;
                }
            return bres;
            }

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            base.DisposeManagedResources();

            // _inifiniteWaitEvent?.Dispose();
            // _inifiniteWaitEvent = null;
            }

        #endregion Реализация интерфейса IDisposable

        /// <summary>
        /// Обработчик событий объектов об изменении состояния
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Actor_StateChangedEvents(object sender, Events.ActorStateChangedEventArgs e)
            {
            if (e is ActorSetCountChangedEventArgs) // события изменения состояния набора не интересны
                {
                return;
                }

            var actor = sender as ActorBase;
            if (actor == null)
                {
                return;
                }

            switch (actor.State)
                {
                case ActorState.Pending:
                    {
                    if (!_waiting.Contains(actor))
                        {
                        MoveToWaiting(actor);
                        }
                    break;
                    }
                case ActorState.Running:
                    {
                    if (!_running.Contains(actor))
                        {
                        MoveToRunning(actor);
                        }
                    break;
                    }
                case ActorState.Started:
                    {
                    if (!_running.Contains(actor))
                        {
                        MoveToRunning(actor);
                        }
                    break;
                    }
                case ActorState.Stopped:
                    {
                    if (actor.RunOnlyOnce)
                        {
                        if (!_completed.Contains(actor))
                            {
                            MoveToCompleted(actor);
                            }
                        }
                    else
                        {
                        if (!_waiting.Contains(actor))
                            {
                            MoveToWaiting(actor);
                            }
                        }
                    break;
                    }
                case ActorState.Terminated:
                    {
                    if (!_completed.Contains(actor))
                        {
                        MoveToCompleted(actor);
                        }
                    break;
                    }
                default:
                    {
                    throw new InvalidOperationException("Непонятное состояние");
                    }
                }
            }

        #region Методы перемещения

        /// <summary>
        /// Переместить в очередь ожидания
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="raiseActorsSetChangedEvent">Отправить событие об изменении состояния набора объектов</param>
        protected void MoveToWaiting(ActorBase actor, bool raiseActorsSetChangedEvent = true)
            {
            lock (Locker)
                {
                if (actor == null)
                    {
                    throw new ArgumentNullException(nameof(actor));
                    }

                if (actor.State == ActorState.Terminated)
                    {
                    throw new InvalidOperationException($"Объект {Name} уже заверщен поэтому объект {actor.Name}  не может быть помещен в cписке ожидания");
                    }

                if (actor.State == ActorState.Running && RunOnlyOnce)
                    {
                    throw new InvalidOperationException($"Объект {Name} уже активен поэтому объект {actor.Name} не может быть помещен в cписке ожидания");
                    }

                if (_waiting.Contains(actor))
                    {
                    //if (raiseActorsSetChangedEvent)
                    //    {
                    //    RaiseActorsSetChanged(actor);
                    //    }
                    return;
                    }

                CheckActorsSetState();

                Interlocked.Increment(ref _waitingCount);
                actor.StateChangedEvents += Actor_StateChangedEvents;
                _waiting.Add(actor);

                if (_running.Contains(actor))
                    {
                    _running.Remove(actor);
                    Interlocked.Decrement(ref _runningCount);
                    }
                if (_completed.Contains(actor))
                    {
                    _completed.Remove(actor);
                    Interlocked.Decrement(ref _completedCount);
                    }

                if (raiseActorsSetChangedEvent)
                    {
                    RaiseActorsSetChanged(actor);
                    }
                } // end lock
            }

        /// <summary>
        /// Переместить в очередь выполнения
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="raiseActorsSetChangedEvent">Отправить событие об изменении состояния набора объектов</param>
        protected void MoveToRunning(ActorBase actor, bool raiseActorsSetChangedEvent = true)
            {
            lock (Locker)
                {
                if (actor == null)
                    {
                    throw new ArgumentNullException(nameof(actor));
                    }

                if (actor.State == ActorState.Terminated)
                    {
                    throw new InvalidOperationException($"Объект {Name} уже заверщен и не может быть помещен в очередь выполнения");
                    }

                if (
                    (actor.State != ActorState.Started) &&
                    (actor.State != ActorState.Running)
                    )
                    {
                    throw new InvalidOperationException($"Объект {Name} не активен");
                    }

                if (_running.Contains(actor))
                    {
                    //if (raiseActorsSetChangedEvent)
                    //    {
                    //    RaiseActorsSetChanged(actor);
                    //    }
                    return;
                    }

                CheckActorsSetState();

                Interlocked.Increment(ref _runningCount);
                _running.Add(actor);

                if (_waiting.Contains(actor))
                    {
                    _waiting.Remove(actor);
                    Interlocked.Decrement(ref _waitingCount);
                    }
                if (_completed.Contains(actor))
                    {
                    _completed.Remove(actor);
                    Interlocked.Decrement(ref _completedCount);
                    }

                if (raiseActorsSetChangedEvent)
                    {
                    RaiseActorsSetChanged(actor);
                    }
                } // end lock
            }

        /// <summary>
        /// Переместить в очередь завершенных объектов
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="raiseActorsSetChangedEvent">Отправить событие об изменении состояния набора объектов</param>
        protected void MoveToCompleted(ActorBase actor, bool raiseActorsSetChangedEvent = true)
            {
            lock (Locker)
                {
                if (actor == null)
                    {
                    throw new ArgumentNullException(nameof(actor));
                    }

                if (actor.State == ActorState.Terminated)
                    {
                    actor.StateChangedEvents -= Actor_StateChangedEvents; // первым делом отписываемся от событий
                    }

                if (_completed.Contains(actor))
                    {
                    //if (raiseActorsSetChangedEvent)
                    //    {
                    //    RaiseActorsSetChanged(actor);
                    //    }
                    return;
                    }

                CheckActorsSetState();

                Interlocked.Increment(ref _completedCount);
                _completed.Add(actor);

                if (_waiting.Contains(actor))
                    {
                    _waiting.Remove(actor);
                    Interlocked.Decrement(ref _waitingCount);
                    }

                if (_running.Contains(actor))
                    {
                    _running.Remove(actor);
                    Interlocked.Decrement(ref _runningCount);
                    }

                if (raiseActorsSetChangedEvent)
                    {
                    RaiseActorsSetChanged(actor);
                    }
                } // end lock
            }

        #endregion Методы перемещения

        #region Вспомогательные методы

        /// <summary>
        /// Добавить объект в список объектов для выполнения
        /// </summary>
        /// <param name="actor">Объект для выполнения</param>
        public void Add(ActorBase actor)
            {
            if (actor == null)
                {
                throw new ArgumentNullException(nameof(actor));
                }

            //    if (PoolClosed)
            //        {
            //        throw new InvalidOperationException("Объект закрыт для добавления новых объектов");
            //        }

            actor.SetParent(this);

            switch (actor.State)
                {
                case ActorState.Pending:
                    {
                    MoveToWaiting(actor);
                    break;
                    }
                case ActorState.Running:
                    {
                    MoveToRunning(actor); // ?
                    break;
                    }
                case ActorState.Started:
                    {
                    MoveToRunning(actor);
                    break;
                    }
                case ActorState.Stopped: // ?
                    {
                    MoveToWaiting(actor);
                    break;
                    }
                case ActorState.Terminated:
                    {
                    MoveToCompleted(actor);
                    break;
                    }
                default:
                    {
                    throw new InvalidOperationException("Непонятное состояние");
                    }
                }
            }

        //    //if (PoolClosed)
        //    //    {
        //    //    throw new InvalidOperationException("Объект закрыт для добавления новых объектов");
        //    //    }

        //    if (IsRunning && (RunningList.Count != 0))
        //        {
        //        throw new InvalidOperationException("Объект уже выполняет работу");
        //        }

        //    if (IsTerminated)
        //        {
        //        throw new InvalidOperationException("Объект уже выполнил всю работу");
        //        }

        //    actor.SetParent(this);
        //    WaitingList.Enqueue(actor);
        //    }

        /// <summary>
        /// Dispose объект если он поддерживает такую возможность
        /// </summary>
        /// <param name="o">Объект</param>
        protected static void DisposeIfPossible(object o)
            {
            switch (o)
                {
                case null:
                    {
                    return;
                    }
                case IDisposable id:
                    {
                    id.Dispose();
                    break;
                    }
                }
            }

        #endregion Вспомогательные методы

        ///// <summary>
        ///// Остановка обработки очереди при возникновении ошибки в работе любого из объектов
        ///// </summary>
        /////private bool _stopPoolOnError = false;

        ///// <summary>
        ///// Вспомогательный объект для создания функции WaitFor()
        ///// </summary>
        //private ManualResetEvent _inifiniteWaitEvent = new ManualResetEvent(false);

        //#region PoolState

        /////// <summary>
        /////// Пул закрыт
        /////// </summary>
        ////private bool _poolClosed;

        /////// <summary>
        /////// Пул закрыт
        /////// </summary>
        ////public bool PoolClosed
        ////    {
        ////    get
        ////        {
        ////        return _poolClosed;
        ////        }
        ////    }

        /////// <summary>
        ///////
        /////// </summary>
        ////protected void ClosePool()
        ////    {
        ////    _poolClosed = true;
        ////    }

        //#endregion PoolState

        //#region Свойства

        ///// <summary>
        ///// Остановка обработки очереди при возникновении ошибки в работе любого из объектов
        ///// </summary>
        ////public bool StopPoolOnError
        ////    {
        ////    get
        ////        {
        ////        return _stopPoolOnError;
        ////        }

        ////    set
        ////        {
        ////        _stopPoolOnError = value;
        ////        }
        ////    }

        ///// <summary>
        ///// Очистить объект от хранимых в нем временных данных, чтобы программа потребляла меньше памяти
        ///// </summary>
        ////public bool CleanupAfterTermination
        ////    {
        ////    get;
        ////    set;
        ////    } = true;

        //#endregion Свойства

        //#region Методы

        ///// <summary>
        ///// Ожидание для группы объектов
        ///// </summary>
        ////protected virtual void WaitForActorsGroup()
        ////    {
        ////    }

        /// <summary>
        /// Прокачать сообщения если поток выполнения - UI
        /// </summary>
        /// <returns>true если поток выполнения - UI</returns>
        //protected static bool DoEvents()
        //    {
        //    return true; //
        //                 //MessagePumpHelper.DoEvents();
        //    }

        /// <summary>
        /// Отменяет выполнение списка объектов
        /// </summary>
        //    public override async Task CancelAsync()
        //        {
        //        await base.CancelAsync();

        //        // ClosePool();

        //        /*switch (Status)
        //{
        //case GuActorExecutionStatus.Pending:
        //case GuActorExecutionStatus.Initialized:
        //	{
        //	// отменяем только ожидающие выполнения объекты выполняющиеся - пусть
        //	// выполняются завершенные - уже завершились
        //	foreach (GuActor actor in WaitingList)
        //		{
        //		actor.Cancel();
        //		}

        //	return;
        //	}
        //        }*/
        //        }

        #region Набор методов для переопределения

        ///// <summary>
        ///// Метод вызывается после отправки сигнала OnActorTerminated и предназначен для очистки
        ///// объекта от хранимых в нем временных данных, чтобы программа потребляла меньше памяти
        ///// </summary>
        //protected override void RunCleanupAfterTermination()
        //	{
        //	if (CleanupAfterTermination)
        //		{
        //		ClearConcurrentQueue<GuActor>(this.WaitingList);
        //		this.RunningList?.Clear();
        //		ClearConcurrentQueue<GuActor>(this.CompletedList);
        //		}

        //	base.RunCleanupAfterTermination();
        //	SoftGarbageCollection("Очистка по завершению '" + Name + "'");
        //	}

        #endregion Набор методов для переопределения
        }
    }