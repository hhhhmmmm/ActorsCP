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
        private IMessageChannel _iMessageChannel;

        #region События объекта

        /// <summary>
        /// Выкинуть событие
        /// </summary>
        /// <param name="actorEventArgs">Событие</param>
        protected void RaiseActorEvent(ActorEventArgs actorEventArgs)
            {
            _events?.Invoke(this, actorEventArgs);
            }

        /// <summary>
        /// Выкинуть событие - изменилось состояние объекта
        /// </summary>
        /// <param name="actorStateChangedEventArgs">Событие</param>
        protected void RaiseActorStateChanged(ActorStateChangedEventArgs actorStateChangedEventArgs)
            {
            _stateChangedEvents?.Invoke(this, actorStateChangedEventArgs);
            }

        /// <summary>
        /// События объекта
        /// </summary>
        private event EventHandler<ActorEventArgs> _events;

        /// <summary>
        /// События объекта
        /// </summary>
        public event EventHandler<ActorEventArgs> Events
            {
            add
                {
                _events += value;
                }
            remove
                {
                _events -= value;
                }
            }

        /// <summary>
        /// События объекта
        /// </summary>
        private event EventHandler<ActorStateChangedEventArgs> _stateChangedEvents;

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        public event EventHandler<ActorStateChangedEventArgs> StateChangedEvents
            {
            add
                {
                _stateChangedEvents += value;
                }
            remove
                {
                _stateChangedEvents -= value;
                }
            }

        #endregion События объекта

        #region Обслуживание IMessageChannel

        /// <summary>
        /// Канал сообщений
        /// </summary>
        public IMessageChannel MessageChannel
            {
            get
                {
                return _iMessageChannel;
                }
            }

        /// <summary>
        /// Установить указатель на канал сообщений
        /// </summary>
        /// <param name="iMessageChannel">Канал сообщений</param>
        public virtual void SetIMessageChannel(IMessageChannel iMessageChannel)
            {
            if (_iMessageChannel == iMessageChannel)
                {
                return;
                }
            if (_iMessageChannel == this)
                {
                return;
                }
            _iMessageChannel = iMessageChannel;
            }

        #endregion Обслуживание IMessageChannel

        #region IMessageChannel

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        public void RaiseDebug(string debugText)
            {
            _iMessageChannel?.RaiseDebug(debugText); // сначала канал а не событие, иначе по завершению вьюпорт отпишется и не будет получать сообщений
            var a = new ActorActionEventArgs(debugText, ActorActionEventType.Debug);
            RaiseActorEvent(a);
            if (Logger.IsDebugEnabled)
                {
                Logger.LogDebug(debugText);
                }
            }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        public void RaiseMessage(string messageText)
            {
            _iMessageChannel?.RaiseMessage(messageText); // сначала канал а не событие, иначе по завершению вьюпорт отпишется и не будет получать сообщений
            var a = new ActorActionEventArgs(messageText, ActorActionEventType.Neutral);
            RaiseActorEvent(a);
            if (Logger.IsInfoEnabled)
                {
                Logger.LogInfo(messageText);
                }
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="warningText">Текст сообщения c предупреждением</param>
        public void RaiseWarning(string warningText)
            {
            _iMessageChannel?.RaiseWarning(warningText); // сначала канал а не событие, иначе по завершению вьюпорт отпишется и не будет получать сообщений
            var a = new ActorActionEventArgs(warningText, ActorActionEventType.Warning);
            RaiseActorEvent(a);
            if (Logger.IsWarnEnabled)
                {
                Logger.LogWarn(warningText);
                }
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="errorText">Текст сообщения об ошибке</param>
        public void RaiseError(string errorText)
            {
            _iMessageChannel?.RaiseError(errorText); // сначала канал а не событие, иначе по завершению вьюпорт отпишется и не будет получать сообщений
            var a = new ActorActionEventArgs(errorText, ActorActionEventType.Error);
            RaiseActorEvent(a);
            if (Logger.IsErrorEnabled)
                {
                Logger.LogError(errorText);
                }
            }

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void RaiseException(Exception exception)
            {
            _iMessageChannel?.RaiseException(exception); // сначала канал а не событие, иначе по завершению вьюпорт отпишется и не будет получать сообщений
            var a = new ActorExceptionEventArgs(exception);
            RaiseActorEvent(a);
            if (Logger.IsErrorEnabled)
                {
                Logger.LogException(exception);
                }
            }

        #endregion IMessageChannel

        #region Генераторы сообщений

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        protected void OnActorAction(string action)
            {
            OnActorAction(action, ActorActionEventType.Neutral);
            }

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить
        /// </summary>
        /// <param name="actions">Набор текстов сообщений</param>
        protected void OnActorAction(params string[] actions)
            {
            foreach (var action in actions)
                {
                OnActorAction(action, ActorActionEventType.Neutral);
                }
            }

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить что-то отладочное
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        protected void OnActorActionDebug(string action)
            {
            OnActorAction("DEBUG: " + action, ActorActionEventType.Debug);
            }

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить что-то системное
        /// </summary>
        /// <param name="actions">Набор текстов сообщений</param>
        protected void OnActorActionDebug(params string[] actions)
            {
            foreach (var action in actions)
                {
                OnActorAction(action, ActorActionEventType.Debug);
                }
            }

        /// <summary>
        /// Событие генерируется когда объект хочет о чем-то предупредить
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        protected void OnActorActionWarning(string action)
            {
            OnActorAction(action, ActorActionEventType.Warning);
            }

        /// <summary>
        /// Событие генерируется когда объект хочет о чем-то предупредить
        /// </summary>
        /// <param name="actions">Набор текстов сообщений</param>
        protected void OnActorActionWarning(params string[] actions)
            {
            foreach (var action in actions)
                {
                OnActorAction(action, ActorActionEventType.Warning);
                }
            }

        /// <summary>
        /// Событие генерируется когда объект хочет сообщить об ошибке
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        protected void OnActorActionError(string action)
            {
            OnActorAction(action, ActorActionEventType.Error);
            }

        /// <summary>
        /// Событие генерируется когда объект хочет сообщить об ошибке
        /// </summary>
        /// <param name="actions">Набор текстов сообщений</param>
        protected void OnActorActionError(params string[] actions)
            {
            foreach (var action in actions)
                {
                OnActorAction(action, ActorActionEventType.Error);
                }
            }

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        /// <param name="EventType">Тип сообщения</param>
        protected void OnActorAction(string action, ActorActionEventType EventType)
            {
            switch (EventType)
                {
                case ActorActionEventType.Debug:
                    {
                    RaiseDebug(action);
                    break;
                    }
                case ActorActionEventType.Neutral:
                    {
                    RaiseMessage(action);
                    break;
                    }
                case ActorActionEventType.Warning:
                    {
                    RaiseWarning(action);
                    break;
                    }
                case ActorActionEventType.Error:
                    {
                    RaiseError(action);
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
        protected void OnActorThrownAnException(Exception exception)
            {
            SetAnErrorOccurred();
            RaiseException(exception);
            }

        #endregion Генераторы сообщений

        #region Методы для вызовы ивентов от имени объекта

        /// <summary>
        /// Событие генерируется когда объект что-то хочет сообщить
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        /// <param name="eventType">Тип сообщения</param>
        public void RaiseOnActorAction(string action, ActorActionEventType eventType)
            {
            OnActorAction(action, eventType);
            }

        /// <summary>
        /// Сообщить что-то отладочное от имени объекта
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        public void RaiseOnActorActionDebug(string action)
            {
            OnActorActionDebug(action);
            }

        /// <summary>
        /// Сообщить что-то от имени объекта
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        public void RaiseOnActorAction(string action)
            {
            OnActorAction(action);
            }

        /// <summary>
        /// Предупредить о чем то от имени объекта
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        public void RaiseOnActorActionWarning(string action)
            {
            OnActorActionWarning(action);
            }

        /// <summary>
        /// Cообщить об ошибке от имени объекта
        /// </summary>
        /// <param name="action">Текст сообщения</param>
        public void RaiseOnActorActionError(string action)
            {
            OnActorActionError(action);
            }

        /// <summary>
        /// Cообщить об возникновении исключения от имени объекта
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void RaiseOnActorThrownAnException(Exception exception)
            {
            OnActorThrownAnException(exception);
            }

        #endregion Методы для вызовы ивентов от имени объекта
        } // end class ActorBase
    } // end namespace ActorsCP.Actors