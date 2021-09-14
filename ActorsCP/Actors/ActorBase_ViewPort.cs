#if DEBUG
#define DEBUG_BIND_UNBIND
#endif // DEBUG

using System;
using System.Collections.Generic;
using System.Threading;

using ActorsCP.Helpers;
using ActorsCP.ViewPorts;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Частичная реализация ActorBase
    /// </summary>
    public partial class ActorBase
        {
        /// <summary>
        /// Очистить все связанное с вьюпортами
        /// </summary>
        private void ClearViewPortHelper()
            {
            if (_viewPortsContainer.IsValueCreated)
                {
                _viewPortsContainer.Value.Dispose();
                _viewPortsContainer = null; // new Lazy<ViewPortsContainer>();
                }
            UnbindAllViewPorts();
            //_iViewPortList.Value?.Clear();
            //_iViewPortList = null;
            }

        /// <summary>
        /// Счетчик подписок/отписок
        /// </summary>
        private int _bindEventsHandlerCounter = 0;

        #region Подписка/отписка на события

        /// <summary>
        /// Привязать обработчики события к объекту
        /// </summary>
        /// <param name="iViewPort">Интерфейс который получает уведомление о подписке на события</param>
        public virtual void BindEventsHandlers(IActorViewPort iViewPort)
            {
            if (iViewPort == null)
                {
                throw new ArgumentNullException(nameof(iViewPort), "iViewPort не может быть null");
                }

            #region Привязываем события объекта к их получателю

            var actorEventsHandler = iViewPort as IActorEventsHandler;

            if (actorEventsHandler != null)
                {
                Events += actorEventsHandler.Actor_Events;
                StateChangedEvents += actorEventsHandler.Actor_StateChangedEvents;
                }

            #endregion Привязываем события объекта к их получателю

            // Сохраняем для возможных потомков порожденных при вызове метода Run()

            _viewPortsContainer.Value.Add(iViewPort);

            // _iViewPortList.Value.Add(new WeakReference(iViewPort, false));

            #region Окончательное уведомление

            var actorBindEventsHandler = iViewPort as IActorBindEventsHandler;
            Interlocked.Increment(ref _bindEventsHandlerCounter);
            actorBindEventsHandler?.Actor_EventHandlersBound(this);

#if DEBUG_BIND_UNBIND
            OnActorActionDebug($"Вызван BindEventsHandlers, _bindEventsHandlerCounter = {_bindEventsHandlerCounter}");
#endif // DEBUG_BIND_UNBIND

            #endregion Окончательное уведомление
            }

        /// <summary>
        /// Отвязать обработчики события от объекта
        /// </summary>
        /// <param name="iViewPort">Объект который получает уведомление об отписке от событий</param>
        public virtual void UnbindEventsHandlers(IActorViewPort iViewPort)
            {
            if (iViewPort == null)
                {
                throw new ArgumentNullException(nameof(iViewPort), "iViewPort не может быть null");
                }

            #region Отвязываем события объекта от их получателя

            var actorEventsHandler = iViewPort as IActorEventsHandler;

            if (actorEventsHandler != null)
                {
                Events -= actorEventsHandler.Actor_Events;
                StateChangedEvents -= actorEventsHandler.Actor_StateChangedEvents;
                }

            #endregion Отвязываем события объекта от их получателя

            #region Окончательное уведомление

            var actorBindEventsHandler = iViewPort as IActorBindEventsHandler;
            Interlocked.Decrement(ref _bindEventsHandlerCounter);
            actorBindEventsHandler?.Actor_EventHandlersUnbound(this);

#if DEBUG_BIND_UNBIND
            OnActorActionDebug($"Вызван UnbindEventsHandlers, _bindEventsHandlerCounter = {_bindEventsHandlerCounter}");
#endif // DEBUG_BIND_UNBIND

            #endregion Окончательное уведомление

            #region Очистка списка - олжна вызываться последней, иначе последнее событие не будет отправлено

            if (_viewPortsContainer.IsValueCreated)
                {
                _viewPortsContainer.Value.DeleteWeakReference(iViewPort);
                }

            //if (_iViewPortList != null && _iViewPortList.Value != null)
            //    {
            //    var list = _iViewPortList.Value;
            //    WeakReferenceHelper.DeleteWeakReference(list, iViewPort);
            //    }

            #endregion Очистка списка - олжна вызываться последней, иначе последнее событие не будет отправлено
            }

        #endregion Подписка/отписка на события

        /// <summary>
        /// Список внешних объектов (реализующих интерфейс IActorViewPort) которые вызвали
        /// метод BindEventsHandlers() Нужен для того, чтобы привязать объекты, созданные при вызове
        /// метода Run() Список дополняется в методе BindEventsHandlers() и освобождается в методе UnbindEventsHandlers()
        /// </summary>
        // private Lazy<List<WeakReference>> _iViewPortList = new Lazy<List<WeakReference>>(() => new List<WeakReference>());

        /// <summary>
        /// Отвязать все вьюпорты от объекта
        /// </summary>
        public virtual void UnbindAllViewPorts()
            {
            if (!_viewPortsContainer.IsValueCreated)
                {
                return;
                }
            //if ((_iViewPortList == null) || (_iViewPortList.Value == null) || (_iViewPortList.Value.Count == 0))
            //    {
            //    return;
            //    }
            var tmpList = _viewPortsContainer.Value.GetCopy();
            // var tmpList = new List<WeakReference>(_iViewPortList.Value);

            foreach (var wr in tmpList)
                {
                if (wr.IsAlive)
                    {
                    var eh = wr.Target as IActorViewPort;
                    if (eh != null)
                        {
                        UnbindEventsHandlers(eh);
                        }
                    } // end IsAlive
                }
            }

        #region Методы для привязки дочерних объектов созданных при вызове метода Run()

        /// <summary>
        /// Для отладки - счетчик вызовов BindChild/UnbindChild
        /// </summary>
        private int _bindChildCounter;

        /// <summary>
        /// Привязка производного объекта, созданного при вызове метода Run()
        /// </summary>
        /// <param name="childActor">Производный объект</param>
        public void BindChild(ActorBase childActor)
            {
            if (childActor == null)
                {
                throw new ArgumentNullException(nameof(childActor));
                }

            if (childActor.Parent != this)
                {
                throw new ArgumentException("Переданный объект не является дочерним");
                }

            lock (Locker)
                {
                foreach (var wr in _viewPortsContainer.Value.ViewPortsList)
                //    foreach (var wr in _iViewPortList.Value)
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
                OnActorActionDebug($"Вызван BindChild, m_BindChildCounter = {_bindChildCounter}");
#endif // DEBUG_BIND_UNBIND
                }  // end lock
            }

        /// <summary>
        /// Отвязка производного объекта от подписчиков
        /// </summary>
        /// <param name="childActor">Производный объект</param>
        public void UnbindChild(ActorBase childActor)
            {
            if (childActor == null)
                {
                throw new ArgumentNullException(nameof(childActor));
                }

            if (childActor.Parent != this)
                {
                throw new ArgumentException("Переданный объект не является дочерним");
                }

            lock (Locker)
                {
#if DEBUG_BIND_UNBIND
                OnActorActionDebug($"Вызван UnbindChild, _bindChildCounter = {_bindChildCounter}");
#endif // DEBUG_BIND_UNBIND
                foreach (var wr in _viewPortsContainer.Value.ViewPortsList)
                //foreach (var wr in _iViewPortList.Value)
                    {
                    if (wr.IsAlive)
                        {
                        var eh = wr.Target as IActorViewPort;
                        if (eh != null)
                            {
                            childActor.UnbindEventsHandlers(eh);
                            }
                        } // end IsAlive
                    } // end foreach

                Interlocked.Decrement(ref _bindChildCounter);
                }  // end lock
            }

        #endregion Методы для привязки дочерних объектов созданных при вызове метода Run()
        } // end class ActorBase
    } // end namespace ActorsCP.Actors