using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

using ActorsCP.Logger;

using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace ActorsCPFormsRunner
    {
    /// <summary>
    /// Релизация логгера через NLog
    /// </summary>
    public partial class ActorLoggerImplementation
        {
        #region Публичные  методы

        /// <summary>
        /// Инициализация логгера
        /// </summary>
        /// <param name="wantedLogFileName">Имя файла лога</param>
        /// <param name="worksUnderWebServer">Работаем под управлением IIS</param>
        /// <returns>true если логгер запустился успешно</returns>
        public bool InitLog(string wantedLogFileName, bool worksUnderWebServer = false)
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
                        InternalRaiseError(e.ToString());
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
                        InternalRaiseWarning(e.ToString());
                        }
                    catch (UnauthorizedAccessException e)
                        {
                        InternalRaiseWarning(e.ToString());
                        }
                    }

                var logLevel = LogLevel.Info;

                LogPath = logPath;

                var config = new LoggingConfiguration();

                CreateLoggingRule(logPath, logFileName, logLevel, out var fileTarget);

                var asyncFileTarget = new AsyncTargetWrapper(fileTarget);

                config.AddTarget("file", asyncFileTarget);
                var fileLoggingRule = new LoggingRule("*", logLevel, asyncFileTarget);
                config.LoggingRules.Add(fileLoggingRule);
                LogManager.ThrowExceptions = true;
                LogManager.Configuration = config;
                SetNlogLogLevel(logLevel);

                _logger = NLog.LogManager.GetCurrentClassLogger();

                if (_logger == null)
                    {
                    return false;
                    }

                _logger.Info($"Команда запуска: {Environment.CommandLine}");
                _logger.Info($"Операционная система: {Environment.OSVersion}");
                _logger.Info($"Версия CLR: {Environment.Version}");
                _logger.Info($"LogLevel: {logLevel.ToString() }");

                return true;
                } // end try
            catch (Exception e)
                {
                InternalRaiseError(e.ToString());
                }
            return false;
            }

        #endregion Публичные  методы

        #region Публичные статические методы

        /// <summary>
        /// Путь к каталогу лога
        /// </summary>
        public static string LogPath
            {
            get;
            private set;
            }

        /// <summary>
        /// Установить обработчики - показыватели сообщений
        /// </summary>
        /// <param name="errorHandler">Обработчик - показыватель ошибок</param>
        /// <param name="warningHandler">Обработчик - показыватель предупреждений</param>
        /// <param name="infoHandler">Обработчик - показыватель сообщений</param>
        public static void SetMessageHandlers(Action<string> errorHandler, Action<string> warningHandler, Action<string> infoHandler)
            {
            _errorHandler = errorHandler;
            _warningHandler = warningHandler;
            _infoHandler = infoHandler;
            }

        /// <summary>
        /// Глобальная настройка NLog
        /// </summary>
        /// <param name="iActorLogger">Настройки созданного логгера</param>
        public static void ConfigureNLogGlobally(IActorLogger iActorLogger)
            {
            if (!iActorLogger.LoggerOptions.IsEnabled)
                {
                SetNlogLogLevel(LogLevel.Off);
                return;
                }

            if (iActorLogger.LoggerOptions.IsDebugEnabled)
                {
                SetNlogLogLevel(LogLevel.Debug);
                }
            if (iActorLogger.LoggerOptions.IsInfoEnabled)
                {
                SetNlogLogLevel(LogLevel.Info);
                }
            if (iActorLogger.LoggerOptions.IsWarnEnabled)
                {
                SetNlogLogLevel(LogLevel.Warn);
                }
            if (iActorLogger.LoggerOptions.IsErrorEnabled)
                {
                SetNlogLogLevel(LogLevel.Error);
                }
            if (iActorLogger.LoggerOptions.IsFatalEnabled)
                {
                SetNlogLogLevel(LogLevel.Fatal);
                }
            }

        #endregion Публичные статические методы

        #region Приватные статические методы

        /// <summary>
        /// Показать ошибку
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        private static void InternalRaiseError(string text)
            {
            _errorHandler?.Invoke(text);
            }

        /// <summary>
        /// Показать предупреждение
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        private static void InternalRaiseWarning(string text)
            {
            _warningHandler?.Invoke(text);
            }

        /// <summary>
        /// Показать сообщение
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        private static void InternalRaiseMessage(string text)
            {
            _infoHandler?.Invoke(text);
            }

        /// <summary>
        /// Reconfigures the NLog logging level.
        /// </summary>
        /// <param name="level">The <see cref="LogLevel" /> to be set.</param>
        private static void SetNlogLogLevel(LogLevel level)
            {
            // Uncomment these to enable NLog logging. NLog exceptions are swallowed by default.
            ////NLog.Common.InternalLogger.LogFile = @"C:\Temp\nlog.debug.log";
            ////NLog.Common.InternalLogger.ActorLogLevel = ActorLogLevel.Debug;

            if (level == LogLevel.Off)
                {
                LogManager.DisableLogging();
                }
            else
                {
                if (!LogManager.IsLoggingEnabled())
                    {
                    LogManager.EnableLogging();
                    }

                foreach (var rule in LogManager.Configuration.LoggingRules)
                    {
                    // Iterate over all levels up to and including the target, (re)enabling them.
                    for (int i = level.Ordinal; i <= 5; i++)
                        {
                        rule.EnableLoggingForLevel(LogLevel.FromOrdinal(i));
                        }
                    }
                }

            LogManager.ReconfigExistingLoggers();
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
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveAboveSize = 25 * 1024 * 1024,
                AutoFlush = true,
                ConcurrentWriteAttempts = 99,
                ConcurrentWrites = false, //  If only one process is going to be writing to the file, consider setting ConcurrentWrites to false for maximum performance.
                BufferSize = 100000,
                KeepFileOpen = true,

                #region Циклическая замена файлов лога

                ArchiveOldFileOnStartup = true,
                //ArchiveNumbering = ArchiveNumberingMode.Sequence,
                ArchiveNumbering = ArchiveNumberingMode.Rolling,
                DeleteOldFileOnStartup = true

                #endregion Циклическая замена файлов лога
                };
            }

        #endregion Приватные статические методы

        #region Приватные статические мемберы

        /// <summary>
        /// Обработчик - показыватель ошибок
        /// </summary>
        private static Action<string> _errorHandler
            {
            get;
            set;
            }

        /// <summary>
        /// Обработчик - показыватель предупреждений
        /// </summary>
        private static Action<string> _warningHandler
            {
            get;
            set;
            }

        /// <summary>
        /// Обработчик - показыватель сообщений
        /// </summary>
        private static Action<string> _infoHandler
            {
            get;
            set;
            }

        #endregion Приватные статические мемберы
        } // end class ActorLoggerImplementation
    } // end namespace ActorsCPConsoleRunner