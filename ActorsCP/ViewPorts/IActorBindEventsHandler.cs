using System;
using System.Collections.Generic;
using System.Text;

using ActorsCP.Actors;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Интерфейс описывает события, возникающие при подписке и отписке
    /// от ивентов при вызове методов BindEventsHandlers() и UnbindEventsHandlers()
    /// </summary>
    public interface IActorBindEventsHandler
        {
        /// <summary>
        /// Вызывается, когда объект подписан на события
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        void Actor_EventHandlersBound(ActorBase actor);

        /// <summary>
        /// Вызывается, когда объект отписан от событий
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        void Actor_EventHandlersUnbound(ActorBase actor);
        } // end interface IActorBindEventsHandler
    } // end namespace ActorsCP.ViewPorts