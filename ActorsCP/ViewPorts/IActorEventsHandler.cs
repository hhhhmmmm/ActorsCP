﻿using System;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Actor_Events(object sender, ActorEventArgs e);

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Actor_StateChangedEvents(object sender, ActorStateChangedEventArgs e);
        } // end interface IActorEventsHandler
    } // end namespace ActorsCP.ViewPorts