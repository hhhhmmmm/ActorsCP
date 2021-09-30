using System;
using System.Collections.Generic;
using System.Text;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Интерфейс описывает события, возникающие при подписке и отписке
    /// от ивентов при вызове методов BindViewPort() и UnbindViewPort()
    /// </summary>
    public interface IActorBindViewPortHandler
        {
        /// <summary>
        /// Вызывается, когда объект подписан на события или отписан от них
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        /// <param name="actorViewPortBoundEventArgs">Событие - объект привязан или отвязан</param>
        void Actor_ViewPortBoundUnbound(ActorBase actor, ActorViewPortBoundEventArgs actorViewPortBoundEventArgs);
        } // end interface IActorBindViewPortHandler
    } // end namespace ActorsCP.ViewPorts