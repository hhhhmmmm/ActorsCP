using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorsCP.dotNET.ViewPorts.TreeViewViewPort
    {
    /// <summary>
    /// Список иконок для дерева
    /// </summary>
    public enum TreeViewImage : int
        {
        /// <summary>
        /// Объект ждет выполнения
        /// </summary>
        ActorWaiting = 0,

        /// <summary>
        /// Объект работает
        /// </summary>
        ActorWorking = 1,

        /// <summary>
        /// Объект успешно завершил работу
        /// </summary>
        ActorCompletedSuccessfully = 2,

        /// <summary>
        /// Объект находится в состоянии ошибки
        /// </summary>
        ActorError = 3,

        /// <summary>
        /// Узел 'События'
        /// </summary>
        ActionNode = 4,

        /// <summary>
        /// Служебная информация, вроде 'Инициализация объекта'
        /// </summary>
        ActionSystemNeutral = 5,

        /// <summary>
        /// Исключение
        /// </summary>
        ActionException = 6,

        /// <summary>
        /// Сообщение - нейтральное сообщение объекта
        /// </summary>
        ActionNeutral = 7,

        /// <summary>
        /// Сообщение - предупреждение объектв
        /// </summary>
        ActionWarning = 8,

        /// <summary>
        /// Сообщение - ошибка объекта
        /// </summary>
        ActionError = 9,
        }
    }