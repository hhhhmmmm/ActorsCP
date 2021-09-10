using System;

namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Интерфейс - канал сообщений
    /// </summary>
    public interface IMessageChannel
        {
        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        void RaiseDebug(string debugText);

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        void RaiseMessage(string messageText);

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="warningText">Текст сообщения c предупреждением</param>
        void RaiseWarning(string warningText);

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="errorText">Текст сообщения об ошибке</param>
        void RaiseError(string errorText);

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        void RaiseException(Exception exception);
        }
    }