using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using ActorsCP.Actors;
using ActorsCP.Actors.Events;

namespace ActorsCP.ViewPorts.ConsoleViewPort
    {
    /// <summary>
    /// Статические свойства и методы для консоли
    /// </summary>
    public static class ConsoleViewPortStatics
        {
        /// <summary>
        /// Локер
        /// </summary>
        private static readonly object Locker = new object();

        #region Свойства

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

        #endregion Свойства

        #region Публичные методы

        /// <summary>
        /// Установить цвет по умолчанию
        /// </summary>
        public static void SetDefaultColor()
            {
            Console.ForegroundColor = MessageColor;
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

        #region Хранилище пробелов для отступов

        /// <summary>
        /// Хранилище пробелов
        /// </summary>
        private static ConcurrentDictionary<int, string> _spaces = new ConcurrentDictionary<int, string>();

        /// <summary>
        /// Вернуть кэшированную строку пробелов для отступов
        /// </summary>
        /// <param name="depth">Глубина отступа</param>
        /// <returns></returns>
        private static string GetDepthString(int depth)
            {
            string str = null;
            if (_spaces.TryGetValue(depth, out str))
                {
                return str;
                }

            str = new string(' ', depth * 4);
            _spaces.TryAdd(depth, str);
            return str;
            }

        #endregion Хранилище пробелов для отступов

        /// <summary>
        /// Отформатировать текст сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string FormatMessageText(ActorBase sender, ActorActionEventArgs action)
            {
            var depthOffset = string.Empty;
            if (sender != null)
                {
                var depth = sender.Depth;
                if (depth != 0)
                    {
                    depthOffset = GetDepthString(depth);
                    }
                }

#if DEBUG
            var str = action.AEA.ToString("D8") + " " + depthOffset + action.EventDateAsString + " " + action.MessageText;
#else
            var str = action.EventDateAsString + " " + depthOffset + action.MessageText;
#endif // DEBUG

            return str;
            }

        /// <summary>
        /// Создать сообщение для вывода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ConsoleMessage CreateMessageFromEventArgs(ActorBase sender, ActorActionEventArgs action)
            {
            ConsoleMessage message = null;
            var messageText = FormatMessageText(sender, action);
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
        /// <param name="actor">Объект</param>
        /// <param name="debugText">Текст отладочного сообщения</param>
        /// <returns></returns>
        public static ConsoleMessage CreateDebugMessage(ActorBase actor, string debugText)
            {
            var actorActionEventArgs = new ActorActionEventArgs(debugText, ActorActionEventType.Debug);
            var message = CreateMessageFromEventArgs(actor, actorActionEventArgs);
            return message;
            }

        /// <summary>
        /// Создать сообщение
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="messageText">Текст сообщения</param>
        /// <returns></returns>
        public static ConsoleMessage CreateMessage(ActorBase actor, string messageText)
            {
            var actorActionEventArgs = new ActorActionEventArgs(messageText, ActorActionEventType.Neutral);
            var message = CreateMessageFromEventArgs(actor, actorActionEventArgs);
            return message;
            }

        /// <summary>
        /// Создать предупреждение
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="warningText">Текст сообщения</param>
        /// <returns></returns>
        public static ConsoleMessage CreateWarningMessage(ActorBase actor, string warningText)
            {
            var actorActionEventArgs = new ActorActionEventArgs(warningText, ActorActionEventType.Warning);
            var message = CreateMessageFromEventArgs(actor, actorActionEventArgs);
            return message;
            }

        /// <summary>
        /// Создать предупреждение
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="errorText">Текст ошибки</param>
        /// <returns></returns>
        public static ConsoleMessage CreateErrorMessage(ActorBase actor, string errorText)
            {
            var actorActionEventArgs = new ActorActionEventArgs(errorText, ActorActionEventType.Error);
            var message = CreateMessageFromEventArgs(actor, actorActionEventArgs);
            return message;
            }

        /// <summary>
        /// Создать сообщение об исключении
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="exception">Исключение</param>
        /// <returns></returns>
        public static ConsoleMessage CreateExceptionMessage(ActorBase actor, Exception exception)
            {
            var actorActionEventArgs = new ActorExceptionEventArgs(exception);
            var message = CreateMessageFromEventArgs(actor, actorActionEventArgs);
            return message;
            }

        /// <summary>
        /// Вывести сообщение асинхронно на экран и перевести строку
        /// </summary>
        /// <param name="consoleMessage">Сообщение</param>
        /// <returns></returns>
        public static Task WriteLineToConsoleAsync(ConsoleMessage consoleMessage)
            {
            return Task.Run(() => ConsoleViewPortStatics.WriteLineToConsole(consoleMessage));
            }

        /// <summary>
        /// Вывести сообщение асинхронно на экран
        /// </summary>
        /// <param name="consoleMessage">Сообщение</param>
        /// <returns></returns>
        public static Task WriteToConsoleAsync(ConsoleMessage consoleMessage)
            {
            return Task.Run(() => WriteToConsoleAsync(consoleMessage));
            }

        #endregion Публичные методы
        } // end class ConsoleViewPortStatics
    } // end namespace ActorsCP.ViewPorts.ConsoleViewPort