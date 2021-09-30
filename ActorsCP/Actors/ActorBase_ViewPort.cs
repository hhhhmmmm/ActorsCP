// #if DEBUG
// #define DEBUG_BIND_UNBIND
// #endif // DEBUG

using System;

using ActorsCP.ViewPorts;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Частичная реализация ActorBase
    /// </summary>
    public partial class ActorBase
        {
        /// <summary>
        /// Контейнер вьюпортов
        /// </summary>
        private ViewPortsContainer _viewPortsContainer;

        #region ClearViewPortHelper

        /// <summary>
        /// Вызов Clear View Port Helper() произведен
        /// </summary>
        private bool _ClearViewPortHelperCalled;

        /// <summary>
        /// Очистить все связанное с вьюпортами
        /// </summary>
        private void ClearViewPortHelper()
            {
            if (_ClearViewPortHelperCalled)
                {
#if DEBUG
                if (_ClearViewPortHelperCalled)
                    {
                    throw new Exception("Повторный вызов Clear View Port Helper()");
                    }
#endif // DEBUG
                return;
                }

            if (_viewPortsContainer != null)
                {
                _viewPortsContainer.Dispose();
                _viewPortsContainer = null;
                }

            _ClearViewPortHelperCalled = true;
            }

        #endregion ClearViewPortHelper

        /// <summary>
        /// Создать контейнер если его не существует
        /// </summary>
        private void CreateViewPortsContainerIfNotExists()
            {
            if (_viewPortsContainer == null)
                {
                _viewPortsContainer = new ViewPortsContainer(this);
                }
            }

        #region Подписка/отписка на события

        /// <summary>
        /// Привязать обработчики события (вьюпорт) к объекту
        /// </summary>
        /// <param name="iViewPort">Интерфейс который получает уведомление о подписке на события</param>
        public virtual void BindViewPort(IActorViewPort iViewPort)
            {
            if (iViewPort == null)
                {
                throw new ArgumentNullException($"{nameof(iViewPort)} не может быть null");
                }

            #region Привязываем события объекта к их получателю

            if (iViewPort is IActorEventsHandler actorEventsHandler)
                {
                Events += actorEventsHandler.Actor_Event;
                StateChangedEvents += actorEventsHandler.Actor_StateChangedEvent;
                }

            #endregion Привязываем события объекта к их получателю

            #region Окончательное уведомление - должно вызываться последней, иначе последнее событие не будет отправлено

            lock (Locker)
                {
                CreateViewPortsContainerIfNotExists();
                _viewPortsContainer.BindViewPortAndNotify(iViewPort);
                }

            #endregion Окончательное уведомление - должно вызываться последней, иначе последнее событие не будет отправлено
            }

        /// <summary>
        /// Отвязать обработчики события (вьюпорт) от объекта
        /// </summary>
        /// <param name="iViewPort">Объект который получает уведомление об отписке от событий</param>
        public virtual void UnbindViewPort(IActorViewPort iViewPort)
            {
            if (iViewPort == null)
                {
                throw new ArgumentNullException($"{nameof(iViewPort)} не может быть null");
                }

            #region Отвязываем события объекта от их получателя

            if (iViewPort is IActorEventsHandler actorEventsHandler)
                {
                Events -= actorEventsHandler.Actor_Event;
                StateChangedEvents -= actorEventsHandler.Actor_StateChangedEvent;
                }

            #endregion Отвязываем события объекта от их получателя

            #region Окончательное уведомление - должно вызываться последней, иначе последнее событие не будет отправлено

#if DEBUG
            if (_viewPortsContainer == null)
                {
                throw new Exception("Попытка отписаться от вьюпорта при отсутствующем контейнере");
                }
#endif // DEBUG

            _viewPortsContainer?.UnbindViewPortAndNotify(iViewPort);

            #endregion Окончательное уведомление - должно вызываться последней, иначе последнее событие не будет отправлено
            }

        #endregion Подписка/отписка на события

        #region Методы для привязки/отвязки дочерних объектов созданных при вызове метода Run() и прочее

        /// <summary>
        /// Отвязать все вьюпорты от объекта
        /// </summary>
        protected void UnbindAllViewPorts() // оригинальный метод для ActorBase
            {
            lock (Locker)
                {
                _viewPortsContainer?.UnbindAllViewPorts();
                }  // end lock
            }

        /// <summary>
        /// Привязка вьюпортов к производному объекту, созданного при вызове метода Run()
        /// </summary>
        /// <param name="childActor">Производный объект</param>
        public void BindViewPortsToChildActor(ActorBase childActor)
            {
            lock (Locker)
                {
                _viewPortsContainer?.BindViewPortsToChildActor(childActor);
                }  // end lock
            }

        /// <summary>
        /// Отвязка производного объекта от подписчиков
        /// </summary>
        /// <param name="childActor">Производный объект</param>
        //public void UnbindChild(ActorBase childActor)
        //    {
        //    lock (Locker)
        //        {
        //        _viewPortsContainer?.UnbindChild(childActor);
        //        }  // end lock
        //    }

        #endregion Методы для привязки/отвязки дочерних объектов созданных при вызове метода Run() и прочее
        } // end class ActorBase
    } // end namespace ActorsCP.Actors