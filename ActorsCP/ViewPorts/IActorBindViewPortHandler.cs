using System;
using System.Collections.Generic;
using System.Text;

using ActorsCP.Actors;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Интерфейс описывает события, возникающие при подписке и отписке
    /// от ивентов при вызове методов BindViewPort() и UnbindViewPort()
    /// </summary>
    public interface IActorBindViewPortHandler
        {
        /// <summary>
        /// Вызывается, когда объект подписан на события
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        void Actor_ViewPortBound(ActorBase actor);

        /// <summary>
        /// Вызывается, когда объект отписан от событий
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        void Actor_ViewPortUnbound(ActorBase actor);
        } // end interface IActorBindViewPortHandler
    } // end namespace ActorsCP.ViewPorts