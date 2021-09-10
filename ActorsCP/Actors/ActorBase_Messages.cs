using System;
using System.Globalization;
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

        #region Генераторы сообщений

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить
        /// </summary>
        /// <param name="Action">Текст сообщения</param>
        protected void OnActorAction(string Action)
            {
            OnActorAction(Action, ActorActionEventType.Neutral);
            }

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить
        /// </summary>
        /// <param name="Actions">Набор текстов сообщений</param>
        protected void OnActorAction(params string[] Actions)
            {
            foreach (var action in Actions)
                {
                OnActorAction(action, ActorActionEventType.Neutral);
                }
            }

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить что-то отладочное
        /// </summary>
        /// <param name="Action">Текст сообщения</param>
        protected void OnActorActionDebug(string Action)
            {
            OnActorAction(Action, ActorActionEventType.Debug);
            }

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить что-то системное
        /// </summary>
        /// <param name="Actions">Набор текстов сообщений</param>
        protected void OnActorActionDebug(params string[] Actions)
            {
            foreach (var action in Actions)
                {
                OnActorAction(action, ActorActionEventType.Debug);
                }
            }

        /// <summary>
        /// Событие генерируется когда объект хочет о чем-то предупредить
        /// </summary>
        /// <param name="Action">Текст сообщения</param>
        protected void OnActorActionWarning(string Action)
            {
            OnActorAction(Action, ActorActionEventType.Warning);
            }

        /// <summary>
        /// Событие генерируется когда объект хочет о чем-то предупредить
        /// </summary>
        /// <param name="Actions">Набор текстов сообщений</param>
        protected void OnActorActionWarning(params string[] Actions)
            {
            foreach (var action in Actions)
                {
                OnActorAction(action, ActorActionEventType.Warning);
                }
            }

        /// <summary>
        /// Событие генерируется когда объект хочет сообщить об ошибке
        /// </summary>
        /// <param name="Action">Текст сообщения</param>
        protected void OnActorActionError(string Action)
            {
            OnActorAction(Action, ActorActionEventType.Error);
            }

        /// <summary>
        /// Событие генерируется когда объект хочет сообщить об ошибке
        /// </summary>
        /// <param name="Actions">Набор текстов сообщений</param>
        protected void OnActorActionError(params string[] Actions)
            {
            foreach (var action in Actions)
                {
                OnActorAction(action, ActorActionEventType.Error);
                }
            }

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить
        /// </summary>
        /// <param name="Action">Текст сообщения</param>
        /// <param name="EventType">Тип сообщения</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        protected void OnActorAction(string Action, ActorActionEventType EventType)
            {
            switch (EventType)
                {
                case ActorActionEventType.Debug:
                    {
                    RaiseDebug(Action);
                    break;
                    }
                case ActorActionEventType.Neutral:
                    {
                    RaiseMessage(Action);
                    break;
                    }
                case ActorActionEventType.Warning:
                    {
                    RaiseWarning(Action);
                    break;
                    }
                case ActorActionEventType.Error:
                    {
                    RaiseError(Action);
                    break;
                    }

                default:
                    {
                    OnActorActionError("Необработанный тип сообщения");
                    throw new ArgumentException("Необработанный тип сообщения");
                    }
                }
            }

        /// <summary>
        /// Событие генерируется при возникновении исключения при вызове какого-либо метода
        /// </summary>
        /// <param name="earray">Массив исключений</param>
        protected virtual void OnActorThrownAnException(params Exception[] earray)
            {
            foreach (Exception e in earray)
                {
                OnActorThrownAnException(e);
                }
            }

        /// <summary>
        /// Событие генерируется при возникновении исключения при вызове какого-либо метода
        /// </summary>
        /// <param name="exception">Исключение</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        protected void OnActorThrownAnException(Exception exception)
            {
            RaiseException(exception);
            }

        #endregion Генераторы сообщений
        } // end class ActorBase
    } // end namespace ActorsCP.Actors