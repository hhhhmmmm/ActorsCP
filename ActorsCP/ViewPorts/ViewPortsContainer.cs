using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using ActorsCP.Actors;
using ActorsCP.Helpers;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Контейнер вьюпортов
    /// </summary>
    public sealed class ViewPortsContainer : DisposableImplementation<ViewPortsContainer>
        {
        #region Приватные мемберы

        /// <summary>
        /// Родительский объект - кто владеет экземпляром этого класса
        /// </summary>
        private ActorBase _parentActor;

        /// <summary>
        /// Список внешних объектов (реализующих интерфейс IActorViewPort) которые вызвали
        /// метод BindViewPortsToChildActor() Нужен для того, чтобы привязать объекты, созданные при вызове
        /// метода Run() Список дополняется в методе BindViewPortsToChildActor() и освобождается в методе UnbindChild()
        /// </summary>
        private readonly List<WeakReference> _iViewPortList = new List<WeakReference>();

        /// <summary>
        /// Счетчик вызовов BindViewPortsToChildActor()
        /// </summary>
        private int _bindViewPortsToChildActorCounter;

        /// <summary>
        /// Счетчик вызовов BindViewPort()/UnbindViewPort()
        /// </summary>
        private int _boundViewPortsCounter;

        #endregion Приватные мемберы

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parentActor">Родительский объект - кто владеет экземпляром этого класса</param>
        public ViewPortsContainer(ActorBase parentActor)
            {
            if (parentActor == null)
                {
                throw new ArgumentNullException(nameof(parentActor));
                }
            _parentActor = parentActor;
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Список вьюпортов
        /// </summary>
        private List<WeakReference> ViewPortsList
            {
            get
                {
                return _iViewPortList;
                }
            }

        /// <summary>
        /// Счетчик вызовов BindViewPortsToChildActor()
        /// </summary>
        public int BindViewPortsToChildActorCounter
            {
            get
                {
                return _bindViewPortsToChildActorCounter;
                }
            }

        /// <summary>
        /// Счетчик вызовов BindViewPortsToChildActor()/UnbindViewPortsToChildActor()
        /// </summary>
        public int BoundViewPortsCounter
            {
            get
                {
                return _boundViewPortsCounter;
                }
            }

        /// <summary>
        /// Список вьюпортов пуст
        /// </summary>
        public bool IsEmpty
            {
            get
                {
                return _iViewPortList.Count == 0;
                }
            }

        #endregion Свойства

        #region Приватные методы

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private List<WeakReference> GetCopy()
            {
            var l = new List<WeakReference>(ViewPortsList);
            return l;
            }

        /// <summary>
        /// Увеличить счетчик вызовов BindViewPort()/UnbindViewPort()
        /// </summary>
        private void IncrementBoundViewPortsCounter()
            {
            Interlocked.Increment(ref _boundViewPortsCounter);
            }

        /// <summary>
        /// Уменьшить счетчик вызовов BindViewPort()/UnbindViewPort()
        /// </summary>
        private void DecrementBoundViewPortsCounter()
            {
            Interlocked.Decrement(ref _boundViewPortsCounter);
            }

        #endregion Приватные методы

        /// <summary>
        /// Удалить вьюпорт из списка
        /// </summary>
        /// <param name="iViewPort">вьюпорт</param>
        public void UnbindViewPortAndNotify(IActorViewPort iViewPort)
            {
            #region Окончательное уведомление

            var actorBindEventsHandler = iViewPort as IActorBindViewPortHandler;
            DecrementBoundViewPortsCounter();
            actorBindEventsHandler?.Actor_ViewPortUnbound(_parentActor);

#if DEBUG_BIND_UNBIND
            _parentActor.RaiseOnActorActionDebug($"Вызван UnbindChild, _bindEventsHandlerCounter = {BindEventsHandlerCounter}");
#endif // DEBUG_BIND_UNBIND

            #endregion Окончательное уведомление

            #region Очистка списка - должна вызываться последней, иначе последнее событие не будет отправлено

            WeakReferenceHelper.DeleteWeakReference(_iViewPortList, iViewPort);

            #endregion Очистка списка - должна вызываться последней, иначе последнее событие не будет отправлено
            }

        /// <summary>
        /// Добавить вьюпорт в список
        /// </summary>
        /// <param name="iViewPort">вьюпорт</param>
        public void BindViewPortAndNotify(IActorViewPort iViewPort)
            {
            _iViewPortList.Add(new WeakReference(iViewPort, false));

            var actorBindEventsHandler = iViewPort as IActorBindViewPortHandler;
            IncrementBoundViewPortsCounter();
            actorBindEventsHandler?.Actor_ViewPortBound(_parentActor);

#if DEBUG_BIND_UNBIND
            _parentActor.RaiseOnActorActionDebug($"Вызван BindViewPortAndNotify, _bindEventsHandlerCounter = {BindEventsHandlerCounter}");
#endif // DEBUG_BIND_UNBIND
            }

        /// <summary>
        /// Привязать к объекту все имеющиеся вьюпорты
        /// </summary>
        /// <param name="childActor">Объект</param>
        public void BindViewPortsToChildActor(ActorBase childActor)
            {
            if (childActor == null)
                {
                throw new ArgumentNullException(nameof(childActor));
                }

            if (childActor.Parent != _parentActor)
                {
                throw new ArgumentException("Переданный объект не является дочерним");
                }

            foreach (var wr in ViewPortsList)
                {
                if (wr.IsAlive)
                    {
                    var eh = wr.Target as IActorViewPort;
                    if (eh != null)
                        {
                        childActor.BindViewPort(eh);
                        }
                    } // end IsAlive
                } // end foreach

            Interlocked.Increment(ref _bindViewPortsToChildActorCounter);

#if DEBUG_BIND_UNBIND
                _parentActor.RaiseOnActorActionDebug($"Вызван BindViewPortsToChildActor, BindViewPortsToChildActorCounter = {BindViewPortsToChildActorCounter}");
#endif // DEBUG_BIND_UNBIND
            }

        /// <summary>
        /// Отвязать все вьюпорты от родительского объекта
        /// </summary>
        public void UnbindAllViewPorts()
            {
            if (IsEmpty)
                {
                return;
                }

            var tmpList = GetCopy();

            foreach (var wr in tmpList)
                {
                if (wr.IsAlive)
                    {
                    var eh = wr.Target as IActorViewPort;
                    if (eh != null)
                        {
                        _parentActor.UnbindViewPort(eh);
                        }
                    } // end IsAlive
                }
            }

        //        /// <summary>
        //        /// Отвязать от объекта все имеющиеся вьюпорты
        //        /// </summary>
        //        /// <param name="childActor">Объект</param>
        //        private void UnbindViewPortsFromChildActor(ActorBase childActor)
        //            {
        //            if (childActor == null)
        //                {
        //                throw new ArgumentNullException(nameof(childActor));
        //                }

        //            if (childActor.Parent != _parentActor)
        //                {
        //                throw new ArgumentException("Переданный объект не является дочерним");
        //                }

        //            foreach (var wr in ViewPortsList)
        //                {
        //                if (wr.IsAlive)
        //                    {
        //                    var eh = wr.Target as IActorViewPort;
        //                    if (eh != null)
        //                        {
        //                        childActor.UnbindViewPort(eh);
        //                        }
        //                    } // end IsAlive
        //                } // end foreach

        //            Interlocked.Decrement(ref _bindViewPortsToChildActorCounter);

        //#if DEBUG_BIND_UNBIND
        //                _parentActor.RaiseOnActorActionDebug($"Вызван UnbindViewPortsFromChildActor, BindViewPortsToChildActorCounter = {BindViewPortsToChildActorCounter}");
        //#endif // DEBUG_BIND_UNBIND
        //            }

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            _parentActor = null;
            _iViewPortList.Clear();
            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable
        }
    }