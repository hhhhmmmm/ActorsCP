﻿using System;
using System.Text;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;
using ActorsCP.Helpers;
using ActorsCP.ViewPorts;

namespace ActorsCP.Tests.ViewPorts
    {
    /// <summary>
    /// Базовый тестовый вьюпорт
    /// </summary>
    public class TestViewPortBase : IActorViewPort, IActorEventsHandler, IActorBindViewPortsHandler
        {
        /// <summary>
        /// Канал сообщений
        /// </summary>
        private IMessageChannel _messageChannel;

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="messageChannel">Канал сообщений</param>
        public TestViewPortBase(IMessageChannel messageChannel)
            {
            _messageChannel = messageChannel;
            }

        #endregion Конструкторы

        #region Реализация интерфейса IActorEventsHandler

        /// <summary>
        /// События объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Actor_Event(object sender, ActorEventArgs e)
            {
            _Counter_Actor_Events++;
            }

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Actor_StateChangedEvent(object sender, ActorStateChangedEventArgs e)
            {
            _Counter_Actor_StateChangedEvents++;
            }

        #endregion Реализация интерфейса IActorEventsHandler

        #region Реализация интерфейса IActorBindEventsHandler

        /// <summary>
        /// Вызывается, когда объект подписан на события
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        public void Actor_ViewPortBound(ActorBase actor)
            {
            _Counter_Actor_EventHandlersBound++;
            }

        /// <summary>
        /// Вызывается, когда объект отписан от событий
        /// </summary>
        /// <param name="actor">Объект типа ActorBase</param>
        public void Actor_ViewPortUnbound(ActorBase actor)
            {
            _Counter_Actor_EventHandlersUnbound++;
            }

        /// <summary>
        /// Счетчик событий  Actor_Event()
        /// </summary>
        public int _Counter_Actor_Events;

        /// <summary>
        /// Счетчик событий  Actor_StateChangedEvent()
        /// </summary>
        public int _Counter_Actor_StateChangedEvents;

        /// <summary>
        /// Счетчик событий  Actor_ViewPortBound
        /// </summary>
        public int _Counter_Actor_EventHandlersBound;

        /// <summary>
        /// Счетчик событий Actor_EventHandlersUnbound
        /// </summary>
        public int _Counter_Actor_EventHandlersUnbound;

        #endregion Реализация интерфейса IActorBindEventsHandler

        //
        } // end class TestViewPortBase
    } // end namespace ActorsCP.Tests.ViewPorts