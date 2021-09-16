using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace ActorsCPConsoleRunner
    {
    /// <summary>
    /// Оболочка для NLog
    /// </summary>
    public class NLogWrapper
        {
        #region Статические мемберы

        /// <summary>
        /// синглтон
        /// </summary>
        private static NLogWrapper s_nLogWrapper;

        /// <summary>
        /// Статический локер
        /// </summary>
        private static readonly object StaticLocker = new object();

        /// <summary>
        /// Обработчик - показыватель ошибок
        /// </summary>
        public static Action<string> ErrorHandler
            {
            get;
            private set;
            }

        /// <summary>
        /// Обработчик - показыватель предупреждений
        /// </summary>
        public static Action<string> WarningHandler
            {
            get;
            private set;
            }

        /// <summary>
        /// Обработчик - показыватель сообщений
        /// </summary>
        public static Action<string> InfoHandler
            {
            get;
            private set;
            }

        /// <summary>
        /// Путь к каталогу лога
        /// </summary>
        public static string LogPath
            {
            get;
            private set;
            }

        #endregion Статические мемберы

        #region Статические методы

        /// <summary>
        /// Получить экземпляр
        /// </summary>
        /// <returns></returns>
        public static NLogWrapper GetInstance()
            {
            lock (StaticLocker)
                {
                if (s_nLogWrapper == null)
                    {
                    s_nLogWrapper = new NLogWrapper();
                    }
                return s_nLogWrapper;
                } // end lock
            }

        /// <summary>
        /// Установить обработчики - показыватели сообщений
        /// </summary>
        /// <param name="errorHandler">Обработчик - показыватель ошибок</param>
        /// <param name="warningHandler">Обработчик - показыватель предупреждений</param>
        /// <param name="infoHandler">Обработчик - показыватель сообщений</param>
        public static void SetMessageHandlers(Action<string> errorHandler, Action<string> warningHandler, Action<string> infoHandler)
            {
            ErrorHandler = errorHandler;
            WarningHandler = warningHandler;
            InfoHandler = infoHandler;
            }

        /// <summary>
        /// Показать ошибку
        /// </summary>
        /// <param name="Text">Текст сообщения</param>
        public static void RaiseError(string Text)
            {
            ErrorHandler?.Invoke(Text);
            }

        /// <summary>
        /// Показать предупреждение
        /// </summary>
        /// <param name="Text">Текст сообщения</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static void RaiseWarning(string Text)
            {
            WarningHandler?.Invoke(Text);
            }

        /// <summary>
        /// Показать сообщение
        /// </summary>
        /// <param name="Text">Текст сообщения</param>
        public static void RaiseMessage(string Text)
            {
            InfoHandler?.Invoke(Text);
            }

        /// <summary>
        /// Создать правило логгирования
        /// </summary>
        /// <param name="logDirectory">Каталог для лога</param>
        /// <param name="logFileName">Имя файла лога</param>
        /// <param name="logLevel">Уровень отладки</param>
        /// <param name="fileTarget">результат</param>
        private static void CreateLoggingRule(string logDirectory, string logFileName, LogLevel logLevel, out FileTarget fileTarget)
            {
            fileTarget = null;
            var fileName = Path.Combine(logDirectory, logFileName);
            var fileNameExt = fileName + ".log";

            fileTarget = new FileTarget
                {
                // Header = Layout.FromString(header),
                CreateDirs = true,
                Layout = Layout.FromString("${date:format=yyyy-MM-dd HH\\:mm\\:ss.fffffff} ${level} ${message}"),
                FileName = fileNameExt,
                Encoding = Encoding.GetEncoding(1251),
                ArchiveFileName = string.Format("{0}.{1}.log", fileName, "{#####}"),
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveAboveSize = 25 * 1024 * 1024,
                AutoFlush = true,
                ConcurrentWriteAttempts = 99,
                ConcurrentWrites = false, //  If only one process is going to be writing to the file, consider setting ConcurrentWrites to false for maximum performance.
                BufferSize = 100000,
                KeepFileOpen = true
                };
            }

        #endregion Статические методы

        #region Mемберы

        /// <summary>
        /// Экземплятор логгера
        /// </summary>
        private Logger s_Logger = LogManager.GetCurrentClassLogger();

        public Logger GetLogger()
            {
            return s_Logger;
            }

        #endregion Mемберы

        #region Mетоды

        /// <summary>
        /// Инициализация логгера
        /// </summary>
        /// <param name="worksUnderWebServer">Работаем под управлением IIS</param>
        /// <param name="wantedLogFileName">Имя файла лога</param>
        /// <param name="debugLevel">Уровень отладки</param>
        /// <returns>true если логгер запустился успешно</returns>
        public bool InitLog(bool worksUnderWebServer, string wantedLogFileName, string debugLevel = null)
            {
            try
                {
                const string LogDirectory = "Log";

                var logFileName = string.Empty;
                var logPath = string.Empty;
                var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                var assemblyLocation = new Uri(executingAssembly.GetName().CodeBase).LocalPath;

                if (worksUnderWebServer) // работаем под IIS
                    {
                    logFileName = wantedLogFileName;
                    var assemblyPath = Path.GetDirectoryName(assemblyLocation);
                    logPath = Path.GetDirectoryName(assemblyPath);
                    if (string.IsNullOrEmpty(logPath))
                        {
                        return false;
                        }
                    logPath = Path.Combine(logPath, LogDirectory);

                    if (!Directory.Exists(logPath))
                        {
                        Directory.CreateDirectory(logPath);
                        }

                    try
                        {
                        var dInfo = new DirectoryInfo(logPath);
                        var dSecurity = dInfo.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dInfo.SetAccessControl(dSecurity);
                        }
                    catch (SecurityException e)
                        {
                        RaiseError(e.ToString());
                        }
                    }
                else // консольный АРМ
                    {
                    logFileName = wantedLogFileName;
                    var assemblyPath = Path.GetDirectoryName(assemblyLocation);
                    logPath = assemblyPath;
                    if (string.IsNullOrEmpty(logPath))
                        {
                        return false;
                        }
                    logPath = Path.Combine(logPath, LogDirectory);

                    if (!Directory.Exists(logPath))
                        {
                        Directory.CreateDirectory(logPath);
                        }

                    // попытка выдать все права на папку Log
                    try
                        {
                        var dInfo = new DirectoryInfo(logPath);
                        var dSecurity = dInfo.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dInfo.SetAccessControl(dSecurity);
                        }
                    catch (SecurityException e)
                        {
                        RaiseWarning(e.ToString());
                        }
                    catch (UnauthorizedAccessException e)
                        {
                        RaiseWarning(e.ToString());
                        }
                    }

                var logLevel = LogLevel.Info;

                if (string.IsNullOrEmpty(debugLevel))
                    {
                    logLevel = LogLevel.Info;

                    if (!worksUnderWebServer)
                        {
                        //if (MessageServerGlobals.DllConfiguration.DumpRequestAndResponse)
                        //	{
                        //	actorLogLevel = ActorLogLevel.Trace;
                        //	}
                        }
                    }
                else
                    {
                    logLevel = LogLevel.FromString(debugLevel);
                    }

                LogPath = logPath;

                var config = new LoggingConfiguration();

                CreateLoggingRule(logPath, logFileName, logLevel, out var fileTarget);

                var asyncFileTarget = new AsyncTargetWrapper(fileTarget);

                config.AddTarget("file", asyncFileTarget);
                var fileLoggingRule = new LoggingRule("*", logLevel, asyncFileTarget);
                config.LoggingRules.Add(fileLoggingRule);
                LogManager.ThrowExceptions = true;
                LogManager.Configuration = config;

                s_Logger = NLog.LogManager.GetCurrentClassLogger();

                if (s_Logger == null)
                    {
                    return false;
                    }

                //s_Logger.
                //NLog.LogManager.GetLogger("aa");

                LogInfo($"ActorLogLevel: {logLevel.ToString() }");
                LogInfo($"Версия CLR: {Environment.Version.ToString()}");
                LogInfo($"Операционная система: {Environment.OSVersion.ToString()}");
                LogInfo($"Команда запуска: {Environment.CommandLine.ToString()}");

                return true;
                } // end try
            catch (Exception e)
                {
                RaiseError(e.ToString());
                }
            return false;
            }

        #endregion Mетоды

        #region Свойства

        /// <summary>
        /// Экземплятор логгера
        /// </summary>
        public NLog.Logger Logger
            {
            get
                {
                return s_Logger;
                }
            }

        /// <summary>
        ///
        /// </summary>
        public bool IsTraceEnabled
            {
            get
                {
                return s_Logger == null ? false : s_Logger.IsTraceEnabled;
                }
            }

        /// <summary>
        ///
        /// </summary>
        public bool IsInfoEnabled
            {
            get
                {
                return s_Logger == null ? false : s_Logger.IsInfoEnabled;
                }
            }

        /// <summary>
        ///
        /// </summary>
        public bool IsWarnEnabled
            {
            get
                {
                return s_Logger == null ? false : s_Logger.IsWarnEnabled;
                }
            }

        /// <summary>
        ///
        /// </summary>
        public bool IsErrorEnabled
            {
            get
                {
                return s_Logger == null ? false : s_Logger.IsErrorEnabled;
                }
            }

        /// <summary>
        ///
        /// </summary>
        public bool IsDebugEnabled
            {
            get
                {
                return s_Logger == null ? false : s_Logger.IsDebugEnabled;
                }
            }

        /// <summary>
        ///
        /// </summary>
        public bool IsFatalEnabled
            {
            get
                {
                return s_Logger == null ? false : s_Logger.IsFatalEnabled;
                }
            }

        #endregion Свойства

        #region Методы логгирования

        /// <summary>
        /// Записать в лог сообщение трассировки
        /// </summary>
        /// <param name="text">Сообщение трассировки</param>
        public void LogTrace(string text)
            {
            if (IsTraceEnabled)
                {
                Logger?.Trace(text);
                }
            }

        /// <summary>
        /// Записать в лог сообщение об ошибке
        /// </summary>
        /// <param name="errorMessage">Поясняющий текст</param>
        /// <param name="title">заголовок письма</param>
        public void LogError(string errorMessage, string title = null)
            {
            if (IsErrorEnabled)
                {
                Logger?.Error(errorMessage);
                }
            }

        /// <summary>
        /// Записать в лог сообщение с предупреждением
        /// </summary>
        /// <param name="warningMessage">Поясняющий текст</param>
        public void LogWarning(string warningMessage)
            {
            if (IsWarnEnabled)
                {
                Logger?.Warn(warningMessage);
                }
            }

        /// <summary>
        /// Записать в лог сообщение
        /// </summary>
        /// <param name="infoMessage">Поясняющий текст</param>
        public void LogInfo(string infoMessage)
            {
            if (IsInfoEnabled)
                {
                //Logger.IsDebugEnabled
                Logger?.Info(infoMessage);
                }
            }

        /// <summary>
        /// Записать в лог сообщение
        /// </summary>
        /// <param name="debugMessage">Поясняющий текст</param>
        public void LogDebug(string debugMessage)
            {
            if (IsDebugEnabled)
                {
                Logger?.Debug(debugMessage);
                }
            }

        /// <summary>
        /// Записать в лог сообщение об исключении
        /// </summary>
        /// <param name="e">Исключение</param>
        /// <param name="message">Поясняющий текст</param>
        public void LogException(Exception e, string message)
            {
            if (IsFatalEnabled)
                {
                if (e == null)
                    {
                    return;
                    }
                Logger?.Fatal(e, message + " " + e);
                }
            }

        /// <summary>
        /// Записать в лог сообщение о наборе исключений
        /// </summary>
        /// <param name="aggregateException">Исключение</param>
        /// <param name="message">Поясняющий текст</param>
        public void LogAggregateException(AggregateException aggregateException, string message)
            {
            if (!IsFatalEnabled)
                {
                return;
                }

            if (aggregateException == null)
                {
                return;
                }

            if (aggregateException.InnerExceptions == null)
                {
                return;
                }

            foreach (Exception exception in aggregateException.InnerExceptions)
                {
                LogException(exception, message);
                }
            }

        #endregion Методы логгирования
        }
    }