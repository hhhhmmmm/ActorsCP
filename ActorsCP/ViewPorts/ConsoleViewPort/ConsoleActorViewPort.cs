using System;
using System.Threading.Tasks;

using ActorsCP.Actors.Events;

namespace ActorsCP.ViewPorts.ConsoleViewPort
    {
    /// <summary>
    /// Консольный вьюпорт для актора
    /// </summary>
    public class ConsoleActorViewPort : ActorViewPortBase
        {
        /// <summary>
        /// Локер
        /// </summary>
        protected static readonly object Locker = new object();

        /// <summary>
        /// Хандлер Ctrl-C
        /// </summary>
        // private static EventHandler m_Control_C_handler;

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ConsoleActorViewPort()
            {
            }

        #endregion Конструкторы

        #region Публичные методы

        /// <summary>
        /// Установить цвет по умолчанию
        /// </summary>
        public void SetDefaultColor()
            {
            Console.ForegroundColor = MessageColor;
            }

        #endregion Публичные методы

        #region Публичные статические методы

        /// <summary>
        /// Восстановить цвета консоли
        /// </summary>
        public static void RestoreColors()
            {
            Console.ResetColor();
            }

        /// <summary>
        /// Вывести сообщение на экран и перевести строку
        /// </summary>
        /// <param name="consoleMessage">Сообщение</param>
        /// <returns></returns>
        public static Task WriteLineToConsoleAsync(ConsoleMessage consoleMessage)
            {
            return Task.Run(() => WriteLineToConsole(consoleMessage));
            }

        /// <summary>
        /// Вывести сообщение на экран
        /// </summary>
        /// <param name="consoleMessage">Сообщение</param>
        /// <returns></returns>
        public static Task WriteToConsoleAsync(ConsoleMessage consoleMessage)
            {
            return Task.Run(() => WriteToConsoleAsync(consoleMessage));
            }

        /// <summary>
        /// Вывести сообщение на экран и перевести строку
        /// </summary>
        /// <param name="consoleMessage">Сообщение</param>
        public static void WriteLineToConsole(ConsoleMessage consoleMessage)
            {
            if (consoleMessage == null)
                {
                return;
                }
            lock (Locker)
                {
                if (consoleMessage.MessageColor != null)
                    {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = (ConsoleColor)consoleMessage.MessageColor;
                    Console.WriteLine(consoleMessage.MessageText);
                    Console.ForegroundColor = color;
                    }
                else
                    {
                    Console.WriteLine(consoleMessage.MessageText);
                    }
                } // end Locker
            }

        /// <summary>
        /// Вывести сообщение на экран
        /// </summary>
        /// <param name="consoleMessage">Сообщение</param>
        public static void WriteToConsole(ConsoleMessage consoleMessage)
            {
            if (consoleMessage == null)
                {
                return;
                }

            lock (Locker)
                {
                if (consoleMessage.MessageColor != null)
                    {
                    var messageColor = (ConsoleColor)consoleMessage.MessageColor;
                    var foregroundColor = Console.ForegroundColor;
                    if (foregroundColor != messageColor)
                        {
                        Console.ForegroundColor = messageColor;
                        Console.Write(consoleMessage.MessageText);
                        Console.ForegroundColor = foregroundColor;
                        }
                    else
                        {
                        Console.Write(consoleMessage.MessageText);
                        }
                    }
                else
                    {
                    Console.Write(consoleMessage.MessageText);
                    }
                } // end Locker
            }

        #endregion Публичные статические методы

        #region Свойства

        /// <summary>
        /// Цвет сообщений консоли для отладочных сообщений
        /// </summary>
        public ConsoleColor DebugColor
            {
            get;
            set;
            } = ConsoleColor.White;

        /// <summary>
        /// Цвет сообщений консоли для сообщений
        /// </summary>
        public ConsoleColor MessageColor
            {
            get;
            set;
            } = ConsoleColor.Green;

        /// <summary>
        /// Цвет сообщений консоли для предупреждений
        /// </summary>
        public ConsoleColor WarningColor
            {
            get;
            set;
            } = ConsoleColor.Yellow;

        /// <summary>
        /// Цвет сообщений консоли для ошибок
        /// </summary>
        public ConsoleColor ErrorColor
            {
            get;
            set;
            } = ConsoleColor.Red;

        /// <summary>
        /// Цвет сообщений консоли для исключений
        /// </summary>
        public ConsoleColor ExceptionColor
            {
            get;
            set;
            } = ConsoleColor.Red;

        #endregion Свойства

        /// <summary>
        ///
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private string FormatMessageText(ActorActionEventArgs action)
            {
            var str = action.EventDateAsString + " " + action.MessageText;
            return str;
            }

        private ConsoleMessage CreateMessage(ActorActionEventArgs action)
            {
            ConsoleMessage message = null;
            var messageText = FormatMessageText(action);
            switch (action.ActionEventType)
                {
                case ActorActionEventType.Debug:
                    {
                    message = new ConsoleMessage(messageText, DebugColor);
                    break;
                    }
                case ActorActionEventType.Error:
                    {
                    message = new ConsoleMessage(messageText, ErrorColor);
                    break;
                    }
                case ActorActionEventType.Exception:
                    {
                    message = new ConsoleMessage(messageText, ExceptionColor);
                    break;
                    }
                case ActorActionEventType.Neutral:
                    {
                    message = new ConsoleMessage(messageText, MessageColor);
                    break;
                    }
                case ActorActionEventType.Warning:
                    {
                    message = new ConsoleMessage(messageText, WarningColor);
                    break;
                    }
                } // end switch
            return message;
            }

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
                var message = CreateMessage(action);
                WriteLineToConsole(message);
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
        } // end class ConsoleActorViewPort
    } // end namespace ActorsCP.ViewPorts.Console