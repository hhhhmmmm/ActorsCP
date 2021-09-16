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
        /// Привязать обработчики события к объекту
        /// </summary>
        /// <param name="iViewPort">Интерфейс который получает уведомление о подписке на события</param>
        public override void BindViewPort(IActorViewPort iViewPort)
            {
            base.BindViewPort(iViewPort);

            // по идее на момент вызова все должны быть здесь
            foreach (var actor in _waiting)
                {
                actor.BindViewPort(iViewPort);
                }

            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _running.Items)
                {
                actor.BindViewPort(iViewPort);
                }

            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _completed.Items)
                {
                actor.BindViewPort(iViewPort);
                }
            }

        /// <summary>
        /// Отвязать обработчики события от объекта
        /// </summary>
        /// <param name="iViewPort">Объект который получает уведомление об отписке от событий</param>
        public override void UnbindViewPort(IActorViewPort iViewPort)
            {
            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _waiting)
                {
                actor.UnbindViewPort(iViewPort);
                }

            // смысла мало (список должен быть пустой), но вызовем
            foreach (var actor in _running.Items)
                {
                actor.UnbindViewPort(iViewPort);
                }

            // по идее на момент вызова все должны быть здесь
            foreach (var actor in _completed.Items)
                {
                actor.UnbindViewPort(iViewPort);
                }

            base.UnbindViewPort(iViewPort);
            }
        } // end class ActorsSet
    } // end namespace ActorsCP.Actors