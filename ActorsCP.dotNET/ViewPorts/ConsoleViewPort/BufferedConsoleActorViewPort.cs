using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Actors.Events;
using ActorsCP.ViewPorts;
using ActorsCP.ViewPorts.ConsoleViewPort;

namespace ActorsCP.dotNET.ViewPorts.ConsoleViewPort
    {
    /// <summary>
    /// Буферизованный консольный вьюпорт для актора
    /// </summary>
    public class BufferedConsoleActorViewPort : BufferedActorViewPortBase
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public BufferedConsoleActorViewPort()
            {
            }

        #endregion Конструкторы

        #region Публичные методы

        /// <summary>
        /// Установить цвет по умолчанию
        /// </summary>
        public void SetDefaultColor()
            {
            ConsoleViewPortStatics.SetDefaultColor();
            }

        /// <summary>
        /// Восстановить цвета консоли
        /// </summary>
        public static void RestoreColors()
            {
            ConsoleViewPortStatics.RestoreColors();
            }

        #endregion Публичные методы

        #region Перегружаемые методы

        /// <summary>
        /// Обработать как ActorStateChangedEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorStateChangedEventArgs(ViewPortItem viewPortItem)
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
                var message = ConsoleViewPortStatics.CreateMessageFromEventArgs(action);
                ConsoleViewPortStatics.WriteLineToConsole(message);
                }
            }

        #endregion Перегружаемые методы
        } // end class BufferedConsoleActorViewPort
    } // end namespace ActorsCP.dotNET.ViewPorts.ConsoleViewPort