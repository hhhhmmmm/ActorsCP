using System;
using System.Text;

namespace ActorsCP.dotNET.ViewPorts.Tree
    {
    /// <summary>
    /// Список иконок для дерева
    /// </summary>
    public enum TreeViewImageIndex : int
        {
        /// <summary>
        /// нет картинки
        /// </summary>
        NoImage = 0,

        #region Сообщения объекта разной важности

        /// <summary>
        /// Узел 'События'
        /// </summary>
        ActionNode,

        /// <summary>
        /// Служебная информация, вроде 'Инициализация объекта'
        /// </summary>
        ActionDebug,

        /// <summary>
        /// Служебная информация, вроде 'Инициализация объекта'
        /// </summary>
        ActionSystemNeutral,

        /// <summary>
        /// Сообщение - нейтральное сообщение объекта
        /// </summary>
        ActionNeutral,

        /// <summary>
        /// Сообщение - предупреждение объектв
        /// </summary>
        ActionWarning,

        /// <summary>
        /// Сообщение - ошибка объекта
        /// </summary>
        ActionError,

        /// <summary>
        /// Исключение
        /// </summary>
        ActionException,

        #endregion Сообщения объекта разной важности

        /// <summary>
        /// Объект ждет выполнения
        /// </summary>
        Actor_Pending,

        /// <summary>
        /// Объект запущен
        /// </summary>
        Actor_Started,

        /// <summary>
        /// Объект работает
        /// </summary>
        Actor_Running,

        /// <summary>
        /// Объект запущен
        /// </summary>
        Actor_Stopped,

        /// <summary>
        /// Объект успешно завершил работу
        /// </summary>
        Actor_Terminated_OK,

        /// <summary>
        /// Объект находится в состоянии ошибки
        /// </summary>
        Actor_Terminated_Failure,

        /// <summary>
        /// Выполнение отменено
        /// </summary>
        Actor_Cancel
        }
    }