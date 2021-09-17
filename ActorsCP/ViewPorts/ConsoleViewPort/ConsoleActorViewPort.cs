using System;
using System.Threading.Tasks;

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
        /// Локер
        /// </summary>
        protected static readonly object Locker = new object();

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
        /// Инициализация вьюпорта
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        public void Init(string additionalText = null)
            {
#if NET461 || NET47 || NETFRAMEWORK
            InitDotNetFramework(additionalText);
#endif // NET461 || NET47
            }

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

        /// <summary>
        /// Отформатировать текст сообщения
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string FormatMessageText(ActorActionEventArgs action)
            {
            var str = action.EventDateAsString + " " + action.MessageText;
            return str;
            }

        /// <summary>
        /// Создать сообщение для вывода
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ConsoleMessage CreateMessageFromEventArgs(ActorActionEventArgs action)
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

        /// <summary>
        /// Создать отладочное сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        /// <returns></returns>
        public static ConsoleMessage CreateDebugMessage(string debugText)
            {
            var actorActionEventArgs = new ActorActionEventArgs(debugText, ActorActionEventType.Debug);
            var message = CreateMessageFromEventArgs(actorActionEventArgs);
            return message;
            }

        /// <summary>
        /// Создать сообщение
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        /// <returns></returns>
        public static ConsoleMessage CreateMessage(string messageText)
            {
            var actorActionEventArgs = new ActorActionEventArgs(messageText, ActorActionEventType.Neutral);
            var message = CreateMessageFromEventArgs(actorActionEventArgs);
            return message;
            }

        /// <summary>
        /// Создать предупреждение
        /// </summary>
        /// <param name="warningText">Текст сообщения</param>
        /// <returns></returns>
        public static ConsoleMessage CreateWarningMessage(string warningText)
            {
            var actorActionEventArgs = new ActorActionEventArgs(warningText, ActorActionEventType.Warning);
            var message = CreateMessageFromEventArgs(actorActionEventArgs);
            return message;
            }

        /// <summary>
        /// Создать предупреждение
        /// </summary>
        /// <param name="errorText">Текст ошибки</param>
        /// <returns></returns>
        public static ConsoleMessage CreateErrorMessage(string errorText)
            {
            var actorActionEventArgs = new ActorActionEventArgs(errorText, ActorActionEventType.Error);
            var message = CreateMessageFromEventArgs(actorActionEventArgs);
            return message;
            }

        /// <summary>
        /// Создать сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        /// <returns></returns>
        public static ConsoleMessage CreateExceptionMessage(Exception exception)
            {
            var actorActionEventArgs = new ActorExceptionEventArgs(exception);
            var message = CreateMessageFromEventArgs(actorActionEventArgs);
            return message;
            }

        #endregion Публичные статические методы

        #region Статические свойства

        /// <summary>
        /// Цвет сообщений консоли для отладочных сообщений
        /// </summary>
        public static ConsoleColor DebugColor
            {
            get;
            set;
            } = ConsoleColor.White;

        /// <summary>
        /// Цвет сообщений консоли для сообщений
        /// </summary>
        public static ConsoleColor MessageColor
            {
            get;
            set;
            } = ConsoleColor.Green;

        /// <summary>
        /// Цвет сообщений консоли для предупреждений
        /// </summary>
        public static ConsoleColor WarningColor
            {
            get;
            set;
            } = ConsoleColor.Yellow;

        /// <summary>
        /// Цвет сообщений консоли для ошибок
        /// </summary>
        public static ConsoleColor ErrorColor
            {
            get;
            set;
            } = ConsoleColor.Red;

        /// <summary>
        /// Цвет сообщений консоли для исключений
        /// </summary>
        public static ConsoleColor ExceptionColor
            {
            get;
            set;
            } = ConsoleColor.Red;

        #endregion Статические свойства

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
                var message = CreateMessageFromEventArgs(action);
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
            var message = CreateDebugMessage(debugText);
            WriteLineToConsole(message);
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
            var message = CreateMessage(messageText);
            WriteLineToConsole(message);
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
            var message = CreateWarningMessage(warningText);
            WriteLineToConsole(message);
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
            var message = CreateErrorMessage(errorText);
            WriteLineToConsole(message);
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
            var message = CreateExceptionMessage(exception);
            WriteLineToConsole(message);
            }

        #endregion Реализация интерфейса IMessageChannel
        } // end class ConsoleActorViewPort
    } // end namespace ActorsCP.ViewPorts.Console