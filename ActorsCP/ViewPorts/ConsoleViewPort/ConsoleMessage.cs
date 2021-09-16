using System;
using System.Text;

namespace ActorsCP.ViewPorts.ConsoleViewPort
    {
    /// <summary>
    /// Сообщение на консоль
    /// </summary>
    public sealed class ConsoleMessage
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="messageText">Текст</param>
        /// <param name="messageColor">Цвет текста</param>
        public ConsoleMessage(string messageText, ConsoleColor? messageColor)
            {
            MessageText = messageText;
            MessageColor = messageColor;
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="messageText">Текст</param>
        public ConsoleMessage(string messageText) : this(messageText, null)
            {
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Цвет текста
        /// </summary>
        public ConsoleColor? MessageColor
            {
            get;
            private set;
            }

        /// <summary>
        /// Текст
        /// </summary>
        public string MessageText
            {
            get;
            private set;
            }

        #endregion Свойства
        }
    } // end namespace ActorsCP.ViewPorts.Console