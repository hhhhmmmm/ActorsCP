using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ActorsCPConsoleRunner.Handlers;

using CommandLine;

namespace ActorsCPConsoleRunner
    {
    internal class MainProgram
        {
        #region Мемберы

        /// <summary>
        /// Первоначальная инициализация приложения выполнена
        /// </summary>
        private static bool m_InitAllCompleted = false;

        /// <summary>
        /// Результат вызова обработчика handler.Run();
        /// </summary>
        private static int HandlerResult = 0;

        /// <summary>
        /// Цвет сообщений консоли для сообщений
        /// </summary>
        private static ConsoleColor? ConsoleMessageColor = ConsoleColor.Green;

        /// <summary>
        /// Цвет сообщений консоли для предупреждений
        /// </summary>
        private static ConsoleColor? ConsoleWarningColor = ConsoleColor.Yellow;

        /// <summary>
        /// Цвет сообщений консоли для ошибок
        /// </summary>
        private static ConsoleColor? ConsoleErrorColor = ConsoleColor.Red;

        /// <summary>
        /// глобальный блокировщик консоли
        /// </summary>
        private static readonly object Locker = new System.Object();

        /// <summary>
        /// Хандлер Ctrl-C
        /// </summary>
        private static EventHandler m_Control_C_handler;

        #endregion Мемберы

        #region Обработчики исключений

        /// <summary>
        /// Обработчик всех необработанных исключений
        /// </summary>
        /// <param name="ex">исключение</param>
        internal static void ReportException(Exception ex)
            {
            if (ex == null)
                {
                return;
                }
            RaiseError("В приложении возникло необработанное исключение");
            RaiseError(ex.ToString());
            }

        /// <summary>
        /// Обработчик необработанных исключений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
            {
            if (e == null)
                {
                return;
                }
            var ex = e.ExceptionObject as Exception;
            ReportException(ex);
            }

        /// <summary>
        /// Обработчик исключений в потоках
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ThreadExceptionTrapper(object sender, ThreadExceptionEventArgs e)
            {
            if (e == null)
                {
                return;
                }
            var ex = e.Exception;
            ReportException(ex);
            }

        /// <summary>
        /// Не работает
        /// </summary>
        private static void InitExceptionHandlers()
            {
            // ловим все не обработанные исключения
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            Application.ThreadException += ThreadExceptionTrapper;
            // Set the unhandled exception mode to force all Windows Forms
            // errors to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            }

        #endregion Обработчики исключений

        #region Реализация интерфейса IMessageChannel

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="debugText">Текст отладочного сообщения</param>
        public static void RaiseDebug(string debugText)
            {
            lock (Locker)
                {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(debugText);
                Console.ForegroundColor = color;
                } // end lock
            }

        /// <summary>
        /// Вывести сообщение об исключении
        /// </summary>
        /// <param name="exception">Исключение</param>
        public static void RaiseException(Exception exception)
            {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(exception.ToString());
            Console.ForegroundColor = color;
            }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="MessageText">Текст сообщения</param>
        public static void RaiseMessage(string MessageText)
            {
            lock (Locker)
                {
                var color = Console.ForegroundColor;
                if (ConsoleMessageColor != null)
                    {
                    Console.ForegroundColor = (ConsoleColor)ConsoleMessageColor;
                    }

                Console.WriteLine(MessageText);

                Console.ForegroundColor = color;
                } // end lock
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="MessageText">Текст предупреждения</param>
        public static void RaiseWarning(string MessageText)
            {
            lock (Locker)
                {
                var color = Console.ForegroundColor;
                if (ConsoleWarningColor != null)
                    {
                    Console.ForegroundColor = (ConsoleColor)ConsoleWarningColor;
                    }

                Console.WriteLine(MessageText);

                Console.ForegroundColor = color;
                } // end lock
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="ErrorText">Текст сообщения об ошибке</param>
        public static void RaiseError(string ErrorText)
            {
            lock (Locker)
                {
                var color = Console.ForegroundColor;

                if (ConsoleErrorColor != null)
                    {
                    Console.ForegroundColor = (ConsoleColor)ConsoleErrorColor;
                    }
                Console.WriteLine(ErrorText);
                Console.ForegroundColor = color;
                } // end lock
            }

        #endregion Реализация интерфейса IMessageChannel

        #region Группа обработки Ctrl-С

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private enum CtrlType
            {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
            }

        /// <summary>
        /// Обработчик нажатия Ctrl-C
        /// </summary>
        /// <param name="sig"></param>
        /// <returns></returns>
        private static bool Control_C_Handler(CtrlType sig)
            {
            RaiseWarning("Завершение приложения по причине внешнего Ctrl-C или других причин");
            RaiseWarning("Приложение завершается");

            RestoreColors();
            Environment.Exit(-1);
            return true;
            }

        #endregion Группа обработки Ctrl-С

        #region Разные методы

        /// <summary>
        /// Возвращает версию файла сборки (та, которая указывается в AssemblyFileVersion в файле AssemblyInfo.cs)
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyFileVersion()
            {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            return version;
            }

        /// <summary>
        /// Установить заголовок
        /// </summary>
        /// <param name="header">Заголовок</param>
        private static void SetTitle(string header)
            {
            var sb = new StringBuilder();

            var str = "Отладчик ActorsCP (v. " + GetAssemblyFileVersion() + ")";
            sb.Append(str);
            sb.Append(" - ");
            sb.Append(header);
            Console.Title = sb.ToString();
            }

        /// <summary>
        /// Восстановить цвета консоли
        /// </summary>
        private static void RestoreColors()
            {
            Console.ResetColor();
            }

        /// <summary>
        /// Первоначальная инициализация при запуске
        /// </summary>
        /// <returns>true если все хорошо</returns>
        internal static bool InitAll()
            {
            if (m_InitAllCompleted)
                {
                return true;
                }

            InitExceptionHandlers();

            try
                {
                // throw new Exception();

                #region Подключаем обработчик Ctrl-C

                m_Control_C_handler += new EventHandler(Control_C_Handler);
                SetConsoleCtrlHandler(m_Control_C_handler, true);

                #endregion Подключаем обработчик Ctrl-C

                //    LoadAppSettings();

                m_InitAllCompleted = true;
                return true;
                } // end try
            catch (Exception ex)
                {
                ReportException(ex);
                }
            return false;
            }

        #endregion Разные методы

        /// <summary>
        /// Основная программа
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
            {
            var bres = InitAll();
            if (!bres)
                {
                return;
                }

            try
                {
                var result = Parser.Default.ParseArguments<QueueHandler, CrowdHandler, DebugHandler, ViewPortHandler>(args);

                if (result.Tag == ParserResultType.NotParsed)
                    {
                    result.WithNotParsed<object>(ErrorHandler);
                    Environment.Exit(-1);
                    }

                result.WithParsed<QueueHandler>((x) => { ConfigureAndRun(x); });
                result.WithParsed<CrowdHandler>((x) => { ConfigureAndRun(x); });
                result.WithParsed<DebugHandler>((x) => { ConfigureAndRun(x); });
                result.WithParsed<ViewPortHandler>((x) => { ConfigureAndRun(x); });
                } // end try
            catch (Exception ex)
                {
                ReportException(ex);
                }
            finally
                {
                RestoreColors();

                // SetTitle("завершен");
                Environment.Exit(HandlerResult);
                }
            }

        #region Обработчики глаголов

        /// <summary>
        /// Настроить и запустить
        /// </summary>
        /// <param name="options"></param>
        private static void ConfigureAndRun(HandlerBase options)
            {
            Console.ForegroundColor = (ConsoleColor)ConsoleMessageColor;
            SetTitle(options.ToString());

            var messageChannelImplementation = new MessageChannelImplementation();
            options.SetIMessageChannel(messageChannelImplementation);
            HandlerResult = options.Run();
            }

        /// <summary>
        /// Обработчик ошибок
        /// </summary>
        /// <param name="errors"></param>
        private static void ErrorHandler(IEnumerable<Error> errors)
            {
            bool stopProcessing = errors.Any(e => e.StopsProcessing);

            if (errors.Any(e => e.Tag == ErrorType.HelpRequestedError || e.Tag == ErrorType.VersionRequestedError))
                {
                return;
                }

            foreach (var error in errors)
                {
                RaiseError(error.ToString());
                }
            }

        #endregion Обработчики глаголов
        }
    }