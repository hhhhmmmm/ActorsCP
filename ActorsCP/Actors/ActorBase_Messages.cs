using System;
using System.Collections.Generic;
using System.Text;

using ActorsCP.Actors.Events;
using ActorsCP.Helpers;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Частичная реализация ActorBase
    /// </summary>
    public partial class ActorBase
        {
        /// <summary>
        /// Канал сообщений
        /// </summary>
        private IMessageChannel m_IMessageChannel;

        #region События объекта

        /// <summary>
        /// Выкинуть событие
        /// </summary>
        /// <param name="actorEventArgs">Событие</param>
        protected void RaiseActorEvent(ActorEventArgs actorEventArgs)
            {
            m_Events?.Invoke(this, actorEventArgs);
            }

        /// <summary>
        /// События объекта
        /// </summary>
        private event EventHandler<ActorEventArgs> m_Events;

        /// <summary>
        /// События объекта
        /// </summary>
        public event EventHandler<ActorEventArgs> Events
            {
            add
                {
                m_Events += value;
                }
            remove
                {
                m_Events -= value;
                }
            }

        #endregion События объекта

        /// <summary>
        /// Установить указатель на канал сообщений
        /// </summary>
        /// <param name="iMessageChannel">Канал сообщений</param>
        public void SetIMessageChannel(IMessageChannel iMessageChannel)
            {
            if (m_IMessageChannel == iMessageChannel)
                {
                return;
                }
            if (m_IMessageChannel == this)
                {
                return;
                }
            m_IMessageChannel = iMessageChannel;
            }

        #region IMessageChannel

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        public void RaiseDebug(string debugText)
            {
            var a = new ActorActionEventArgs(debugText, ActorActionEventType.Debug);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseDebug(debugText);
            }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        public void RaiseMessage(string messageText)
            {
            var a = new ActorActionEventArgs(messageText, ActorActionEventType.Neutral);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseMessage(messageText);
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="warningText">Текст сообщения c предупреждением</param>
        public void RaiseWarning(string warningText)
            {
            var a = new ActorActionEventArgs(warningText, ActorActionEventType.Warning);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseWarning(warningText);
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="errorText">Текст сообщения об ошибке</param>
        public void RaiseError(string errorText)
            {
            var a = new ActorActionEventArgs(errorText, ActorActionEventType.Error);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseError(errorText);
            }

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void RaiseException(Exception exception)
            {
            var a = new ActorExceptionEventArgs(exception);
            RaiseActorEvent(a);
            m_IMessageChannel?.RaiseException(exception);
            }

        #endregion IMessageChannel
        } // end class ActorBase
    } // end namespace ActorsCP.Actors