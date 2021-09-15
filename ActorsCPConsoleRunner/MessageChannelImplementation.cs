using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Helpers;

namespace ActorsCPConsoleRunner
    {
    /// <summary>
    /// Вспомогательная реализация интерфейса IMessageChannel
    /// </summary>
    public class MessageChannelImplementation : IMessageChannel
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public MessageChannelImplementation()
            {
            }

        #region Реализация интерфейса IMessageChannel

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="MessageText">Текст сообщения</param>
        public void RaiseMessage(string MessageText)
            {
            MainProgram.RaiseMessage(MessageText);
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="MessageText">Текст сообщения</param>
        public void RaiseWarning(string MessageText)
            {
            MainProgram.RaiseWarning(MessageText);
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="ErrorText">Текст сообщения об ошибке</param>
        public void RaiseError(string ErrorText)
            {
            MainProgram.RaiseError(ErrorText);
            }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        public void RaiseDebug(string debugText)
            {
            MainProgram.RaiseDebug(debugText);
            }

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void RaiseException(Exception exception)
            {
            MainProgram.RaiseException(exception);
            }

        #endregion Реализация интерфейса IMessageChannel

        #endregion Конструкторы
        } // end class MessageChannelImplementation
    } // end namespace ActorsCPConsoleRunner