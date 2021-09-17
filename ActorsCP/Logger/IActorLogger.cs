using System;
using System.Text;

namespace ActorsCP.Logger
    {
    /// <summary>
    /// Интерфейс логгера
    /// </summary>
    public interface IActorLogger
        {
        #region Свойства

        /// <summary>
        /// Уровень подробности логгера
        /// </summary>
        ActorLogLevel Level
            {
            get;
            }

        /// <summary>
        /// Логгер включен
        /// </summary>
        bool IsEnabled
            {
            get;
            }

        /// <summary>
        /// Фатальные ошибки включены
        /// </summary>
        bool IsFatalEnabled
            {
            get;
            }

        /// <summary>
        /// Ошибки включены
        /// </summary>
        bool IsErrorEnabled
            {
            get;
            }

        /// <summary>
        /// Предупреждения включены
        /// </summary>
        bool IsWarnEnabled
            {
            get;
            }

        /// <summary>
        /// Информация включена
        /// </summary>
        bool IsInfoEnabled
            {
            get;
            }

        /// <summary>
        /// Уровень отладки включен
        /// </summary>
        bool IsDebugEnabled
            {
            get;
            }

        #endregion Свойства

        #region Методы логгера

        /// <summary>
        /// Фатальная ошибка
        /// </summary>
        /// <param name="fatalText">Текст фатальной ошибки</param>
        void LogFatal(string fatalText);

        /// <summary>
        /// Исключение
        /// </summary>
        /// <param name="exception">Исключение</param>
        void LogException(Exception exception);

        /// <summary>
        /// Ошибка
        /// </summary>
        /// <param name="errorText">Текст ошибки</param>
        void LogError(string errorText);

        /// <summary>
        /// Предупреждение
        /// </summary>
        /// <param name="warnText">Текст предупреждения</param>
        void LogWarn(string warnText);

        /// <summary>
        /// Информация
        /// </summary>
        /// <param name="infoText">Текст информации</param>
        void LogInfo(string infoText);

        /// <summary>
        /// Отладка
        /// </summary>
        /// <param name="debugText">Текст отладки</param>
        void LogDebug(string debugText);

        #endregion Методы логгера
        }
    }