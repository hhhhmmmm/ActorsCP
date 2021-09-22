using System;

using ActorsCP.Actors.Events;
using ActorsCP.Helpers;

namespace ActorsCP.ViewPorts.ConsoleViewPort
    {
    /// <summary>
    /// Консольный вьюпорт для актора
    /// </summary>
    public partial class ConsoleActorViewPort : ActorViewPortBase, IMessageChannel
        {
        /// <summary>
        /// Инициализация вьюпорта
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        protected override void InternalInit(string additionalText)
            {
            ConsoleViewPortStatics.SetDefaultColor();

#if NET461 || NET47 || NETFRAMEWORK
            InitDotNetFramework(additionalText);
#endif // NET461 || NET47
            }

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            ConsoleViewPortStatics.RestoreColors();
            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable

        #region Перегружаемые методы IActorEventsHandler

        /// <summary>
        /// События объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        protected override void InternalActor_Event(object sender, ActorEventArgs e)
            {
            if (e is ActorActionEventArgs action)
                {
                var message = ConsoleViewPortStatics.CreateMessageFromEventArgs(action);
                ConsoleViewPortStatics.WriteLineToConsole(message);
                }
            }

        /// <summary>
        /// События - изменилось состояние объекта
        /// </summary>
        /// <param name="sender">Отправитель - объект</param>
        /// <param name="e">Событие</param>
        protected override void InternalActor_StateChangedEvent(object sender, ActorStateChangedEventArgs e)
            {
            }

        #endregion Перегружаемые методы IActorEventsHandler

        #region Реализация интерфейса IMessageChannel

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        public void RaiseDebug(string debugText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var message = ConsoleViewPortStatics.CreateDebugMessage(debugText);
            ConsoleViewPortStatics.WriteLineToConsole(message);
            }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        public void RaiseMessage(string messageText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var message = ConsoleViewPortStatics.CreateMessage(messageText);
            ConsoleViewPortStatics.WriteLineToConsole(message);
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="warningText">Текст сообщения</param>
        public void RaiseWarning(string warningText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var message = ConsoleViewPortStatics.CreateWarningMessage(warningText);
            ConsoleViewPortStatics.WriteLineToConsole(message);
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="errorText">Текст сообщения об ошибке</param>
        public void RaiseError(string errorText)
            {
            if (NoOutMessages)
                {
                return;
                }
            var message = ConsoleViewPortStatics.CreateErrorMessage(errorText);
            ConsoleViewPortStatics.WriteLineToConsole(message);
            }

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void RaiseException(Exception exception)
            {
            if (NoOutMessages)
                {
                return;
                }
            var message = ConsoleViewPortStatics.CreateExceptionMessage(exception);
            ConsoleViewPortStatics.WriteLineToConsole(message);
            }

        #endregion Реализация интерфейса IMessageChannel
        } // end class ConsoleActorViewPort
    } // end namespace ActorsCP.ViewPorts.Console