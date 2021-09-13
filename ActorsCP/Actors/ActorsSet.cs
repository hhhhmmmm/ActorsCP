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

        /// <summary>
        /// Очистить объект от хранимых в нем временных данных,
        /// чтобы программа потребляла меньше памяти
        /// Фактически - не добавлять объект в список завершенных
        /// </summary>
        public bool CleanupAfterTermination
            {
            get;
            protected set;
            } = true;

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

        #region Приватные методы

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

        #endregion Приватные методы

        #region Перегруженные методы

        /// <summary>
        /// Метод вызывается до отправки сигнала OnActorTerminated и предназначен для очистки объекта
        /// от хранимых в нем временных данных. Также вызывается из Dispose()
        /// </summary>
        /// <param name="fromDispose">Вызов из Dispose()</param>
        protected override async Task<bool> InternalRunCleanupBeforeTerminationAsync(bool fromDispose)
            {
            var bres = await base.InternalRunCleanupBeforeTerminationAsync(fromDispose);

            lock (Locker)
                {
                _waiting?.Clear();
                _waitingCount = 0;
                _running?.Clear();
                _runningCount = 0;
                if (fromDispose)
                    {
                    _completed?.Clear();
                    _completedCount = 0;
                    }
                }
            // SoftGarbageCollection("Очистка по завершению '" + Name + "'");
            return bres;
            }

        /// <summary>
        /// Установить родительский объект
        /// </summary>
        /// <param name="parentActor">Родительский объект</param>
        public override void SetParent(ActorBase parentActor)
            {
            lock (Locker)
                {
                base.SetParent(parentActor);

                foreach (var actor in _waiting)
                    {
                    actor.SetParent(parentActor);
                    }

                foreach (var actor in _running.Keys)
                    {
                    actor.SetParent(parentActor);
                    }
                }
            }

        /// <summary>
        /// Отменяет выполнение списка объектов
        /// </summary>
        public override async Task CancelAsync()
            {
            await base.CancelAsync();

            foreach (var actor in _waiting)
                {
                await actor.CancelAsync();
                }

            foreach (var actor in _running.Keys)
                {
                await actor.CancelAsync();
                }
            }

        #endregion Перегруженные методы

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

        #region Обработчики событий

        /// <summary>
        /// Обработчик событий объектов об изменении состояния
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Actor_StateChangedEvents(object sender, Events.ActorStateChangedEventArgs e)
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
                            if (!(actor.State == ActorState.Terminated))
                                {
                                await actor.TerminateAsync();
                                }
                            else
                                {
                                MoveToCompleted(actor);
                                }
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

        #endregion Обработчики событий

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

                if (actor.State == ActorState.Terminated)
                    {
                    Interlocked.Increment(ref _completedCount);
                    }

                if (CleanupAfterTermination) // если флаг CleanupAfterTermination установлен, то сразу выбрасываем объект
                    {
                    actor.Dispose(); // Вызываем Dispose() для actor
                    }
                else
                    {
                    _completed.Add(actor); // кладем в список завершенных
                    }

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

        /// <summary>
        /// Установить флаг автоматической очистки после завершения объекта
        /// </summary>
        /// <param name="cleanupAfterTermination">Если true, то по завершении выполнения
        /// объекта он не будет помещаться в очередь и для него будет вызван Dispose()</param>
        public void SetCleanupAfterTermination(bool cleanupAfterTermination)
            {
            CleanupAfterTermination = cleanupAfterTermination;
            }

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
        }
    }

///// <summary>
///// Остановка обработки очереди при возникновении ошибки в работе любого из объектов
///// </summary>
/////private bool _stopPoolOnError = false;

///// <summary>
///// Вспомогательный объект для создания функции WaitFor()
///// </summary>
//private ManualResetEvent _inifiniteWaitEvent = new ManualResetEvent(false);

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