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
        /// метод BindChild() Нужен для того, чтобы привязать объекты, созданные при вызове
        /// метода Run() Список дополняется в методе BindChild() и освобождается в методе UnbindChild()
        /// </summary>
        private readonly List<WeakReference> _iViewPortList = new List<WeakReference>();

        /// <summary>
        /// Счетчик вызовов BindChild()
        /// </summary>
        private int _bindChildCounter;

        /// <summary>
        /// Счетчик вызовов BindChild()/UnbindChild()
        /// </summary>
        private int _bindEventsHandlerCounter;

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
        /// Счетчик вызовов BindChild()
        /// </summary>
        public int BindChildCounter
            {
            get
                {
                return _bindChildCounter;
                }
            }

        /// <summary>
        /// Счетчик вызовов BindChild()/UnbindChild()
        /// </summary>
        public int BindEventsHandlerCounter
            {
            get
                {
                return _bindEventsHandlerCounter;
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

        /// <summary>
        /// Увеличить счетчик вызовов BindChild()/UnbindChild()
        /// </summary>
        public void IncrementBindEventsHandlerCounter()
            {
            Interlocked.Increment(ref _bindEventsHandlerCounter);
            }

        /// <summary>
        /// Уменьшить счетчик вызовов BindChild()/UnbindChild()
        /// </summary>
        public void DecrementBindEventsHandlerCounter()
            {
            Interlocked.Decrement(ref _bindEventsHandlerCounter);
            }

        /// <summary>
        /// Удалить вьюпорт из списка
        /// </summary>
        /// <param name="iViewPort">вьюпорт</param>
        public void RemoveAndNotify(IActorViewPort iViewPort)
            {
            #region Окончательное уведомление

            var actorBindEventsHandler = iViewPort as IActorBindEventsHandler;
            DecrementBindEventsHandlerCounter();
            actorBindEventsHandler?.Actor_EventHandlersUnbound(_parentActor);

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
        public void AddAndNotify(IActorViewPort iViewPort)
            {
            _iViewPortList.Add(new WeakReference(iViewPort, false));

            var actorBindEventsHandler = iViewPort as IActorBindEventsHandler;
            IncrementBindEventsHandlerCounter();
            actorBindEventsHandler?.Actor_EventHandlersBound(_parentActor);

#if DEBUG_BIND_UNBIND
            _parentActor.RaiseOnActorActionDebug($"Вызван BindChild, _bindEventsHandlerCounter = {BindEventsHandlerCounter}");
#endif // DEBUG_BIND_UNBIND
            }

        public List<WeakReference> GetCopy()
            {
            var l = new List<WeakReference>(ViewPortsList);
            return l;
            }

        /// <summary>
        /// Привязать к объекту все имеющиеся вьюпорты
        /// </summary>
        /// <param name="childActor">Объект</param>
        public void BindChild(ActorBase childActor)
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
                        childActor.BindEventsHandlers(eh);
                        }
                    } // end IsAlive
                } // end foreach

            Interlocked.Increment(ref _bindChildCounter);

#if DEBUG_BIND_UNBIND
                _parentActor.RaiseOnActorActionDebug($"Вызван BindChild, _bindChildCounter = {BindChildCounter}");
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
                        _parentActor.UnbindEventsHandlers(eh);
                        }
                    } // end IsAlive
                }
            }

        //        /// <summary>
        //        /// Отвязать от объекта все имеющиеся вьюпорты
        //        /// </summary>
        //        /// <param name="childActor">Объект</param>
        //        private void UnbindChild(ActorBase childActor)
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
        //                        childActor.UnbindEventsHandlers(eh);
        //                        }
        //                    } // end IsAlive
        //                } // end foreach

        //            Interlocked.Decrement(ref _bindChildCounter);

        //#if DEBUG_BIND_UNBIND
        //                _parentActor.RaiseOnActorActionDebug($"Вызван UnbindChild, BindChildCounter = {BindChildCounter}");
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