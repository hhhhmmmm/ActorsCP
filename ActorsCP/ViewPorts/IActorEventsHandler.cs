using System;
using System.Text;

using ActorsCP.Actors.Events;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Интерфейс для привязки событий возникающих при работе объекта Actor
    /// </summary>
    public interface IActorEventsHandler
        {
        /// <summary>
        /// События объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        void Actor_Event(object sender, ActorEventArgs e);

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        void Actor_StateChangedEvent(object sender, ActorStateChangedEventArgs e);
        } // end interface IActorEventsHandler
    } // end namespace ActorsCP.ViewPorts