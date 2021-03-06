using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ActorsCP.Actors.Events;
using ActorsCP.ViewPorts;
using ActorsCP.ViewPorts.ConsoleViewPort;

namespace ActorsCP.dotNET.ViewPorts.Console
    {
    /// <summary>
    /// Буферизованный консольный вьюпорт для актора
    /// </summary>
    public sealed class ConsoleViewPort : TplBufferedActorViewPortBase
        {
        /// <summary>
        /// Инициализация для .Net framework
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        private void InitDotNetFramework(string additionalText = null)
            {
            SetTitle(additionalText);

            #region Подключаем обработчик Ctrl-C

            _Control_C_handler += new EventHandler(Control_C_Handler);
            SetConsoleCtrlHandler(_Control_C_handler, true);

            #endregion Подключаем обработчик Ctrl-C

            InitExceptionHandlers();
            }

        #region Обработчики исключений

        /// <summary>
        /// Обработчик всех необработанных исключений
        /// </summary>
        /// <param name="ex">исключение</param>
        public static void ReportException(Exception ex)
            {
            if (ex == null)
                {
                return;
                }
            var message = ConsoleViewPortStatics.CreateErrorMessage("В приложении возникло необработанное исключение");
            ConsoleViewPortStatics.WriteLineToConsole(message);
            message = ConsoleViewPortStatics.CreateExceptionMessage(ex);
            ConsoleViewPortStatics.WriteLineToConsole(message);
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

        /// <summary>
        /// Хандлер Ctrl-C
        /// </summary>
        private static EventHandler _Control_C_handler;

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
            var message = ConsoleViewPortStatics.CreateWarningMessage("Завершение приложения по причине внешнего Ctrl-C или других причин");
            ConsoleViewPortStatics.WriteLineToConsole(message);
            var bres = ExitInstance(-1);
            return bres;
            }

        /// <summary>
        /// Завершение приложения
        /// </summary>
        /// <param name="exitCode">Код завершения</param>
        public static bool ExitInstance(int exitCode)
            {
            var message = ConsoleViewPortStatics.CreateWarningMessage("Приложение завершило работу.");
            ConsoleViewPortStatics.WriteLineToConsole(message);
            Environment.Exit(exitCode);
            return true;
            }

        #endregion Группа обработки Ctrl-С

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            base.DisposeManagedResources();
            ConsoleViewPortStatics.RestoreColors();
            }

        #endregion Реализация интерфейса IDisposable

        #region Перегружаемые методы

        /// <summary>
        /// Инициализация для .Net framework
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        protected override void InternalInit(string additionalText = null)
            {
            InitDotNetFramework(additionalText);
            base.InternalInit(additionalText);
            }

        /// <summary>
        /// Установить заголовок
        /// </summary>
        /// <param name="additionalText">Заголовок</param>
        public override void InternalSetTitle(string additionalText = null)
            {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var fileVersion = fvi.FileVersion;
            var fileDescription = fvi.FileDescription;

            var str = $"{fileDescription} (v. " + fileVersion + ")";
            if (!string.IsNullOrEmpty(additionalText))
                {
                str = str + " " + additionalText;
                }

            System.Console.Title = str;
            }

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