using System;
using System.Text;

using ActorsCP.ViewPorts;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Частичная реализация ActorsSet
    /// </summary>
    public partial class ActorsSet
        {
        /// <summary>
        /// Отвязать все вьюпорты от объекта
        /// </summary>
        public override void UnbindAllViewPorts()
            {
            base.UnbindAllViewPorts();

            // по идее на момент вызова все должны быть здесь
            foreach (var actor in _waiting)
                {
                actor.UnbindAllViewPorts();
                }

            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _running.Items)
                {
                actor.UnbindAllViewPorts();
                }

            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _completed.Items)
                {
                actor.UnbindAllViewPorts();
                }
            }

        /// <summary>
        /// Привязать обработчики события к объекту
        /// </summary>
        /// <param name="iViewPort">Интерфейс который получает уведомление о подписке на события</param>
        public override void BindEventsHandlers(IActorViewPort iViewPort)
            {
            base.BindEventsHandlers(iViewPort);

            // по идее на момент вызова все должны быть здесь
            foreach (var actor in _waiting)
                {
                actor.BindEventsHandlers(iViewPort);
                }

            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _running.Items)
                {
                actor.BindEventsHandlers(iViewPort);
                }

            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _completed.Items)
                {
                actor.BindEventsHandlers(iViewPort);
                }
            }

        /// <summary>
        /// Отвязать обработчики события от объекта
        /// </summary>
        /// <param name="iViewPort">Объект который получает уведомление об отписке от событий</param>
        public override void UnbindEventsHandlers(IActorViewPort iViewPort)
            {
            base.UnbindEventsHandlers(iViewPort);

            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _waiting)
                {
                actor.UnbindEventsHandlers(iViewPort);
                }

            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _running.Items)
                {
                actor.UnbindEventsHandlers(iViewPort);
                }

            // по идее на момент вызова все должны быть здесь
            foreach (var actor in _completed.Items)
                {
                actor.UnbindEventsHandlers(iViewPort);
                }
            }
        } // end class ActorsSet
    } // end namespace ActorsCP.Actors