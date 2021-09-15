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
        public bool RaiseMessages
            {
            get;
            set;
            } = true;

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
        /// <param name="messageText">Текст сообщения</param>
        public void RaiseMessage(string messageText)
            {
            if (!RaiseMessages)
                {
                return;
                }
            MainProgram.RaiseMessage(messageText);
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="warningText">Текст сообщения</param>
        public void RaiseWarning(string warningText)
            {
            if (!RaiseMessages)
                {
                return;
                }
            MainProgram.RaiseWarning(warningText);
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="errorText">Текст сообщения об ошибке</param>
        public void RaiseError(string errorText)
            {
            if (!RaiseMessages)
                {
                return;
                }
            MainProgram.RaiseError(errorText);
            }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        public void RaiseDebug(string debugText)
            {
            if (!RaiseMessages)
                {
                return;
                }
            MainProgram.RaiseDebug(debugText);
            }

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void RaiseException(Exception exception)
            {
            if (!RaiseMessages)
                {
                return;
                }
            MainProgram.RaiseException(exception);
            }

        #endregion Реализация интерфейса IMessageChannel

        #endregion Конструкторы
        } // end class MessageChannelImplementation
    } // end namespace ActorsCPConsoleRunner