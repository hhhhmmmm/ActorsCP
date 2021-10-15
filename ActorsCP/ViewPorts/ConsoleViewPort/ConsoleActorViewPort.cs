using System;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;

namespace ActorsCP.ViewPorts.ConsoleViewPort
    {
    /// <summary>
    /// Консольный вьюпорт для актора
    /// </summary>
    public partial class ConsoleActorViewPort : QueueBufferedActorViewPortBase
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

            base.InternalInit(additionalText);
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
                var actor = sender as ActorBase;
                var message = ConsoleViewPortStatics.CreateMessageFromEventArgs(actor, action);
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

        /// <summary>
        /// Обработать как ActorEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorEventArgs(ViewPortItem viewPortItem)
            {
            if (viewPortItem.ActorEventArgs is ActorActionEventArgs action)
                {
                var message = ConsoleViewPortStatics.CreateMessageFromEventArgs(viewPortItem.Sender, action);
                ConsoleViewPortStatics.WriteLineToConsole(message);
                }
            }

        #endregion Перегружаемые методы IActorEventsHandler
        } // end class ConsoleActorViewPort
    } // end namespace ActorsCP.ViewPorts.Console