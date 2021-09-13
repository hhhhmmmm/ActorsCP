using System;
using System.Text;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Состояние объекта
    /// </summary>
    public enum ActorState
        {
        /// <summary>
        /// Ожидание запуска - начальное состояние
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Переведен в состояние работы
        /// </summary>
        ///
        Started = 1,

        /// <summary>
        /// Работает
        /// </summary>

        Running = 2,

        /// <summary>
        /// Остановлен
        /// </summary>
        Stopped = 3,

        /// <summary>
        /// Завершен навсегда
        /// </summary>
        Terminated = 4
        } // end enum ActorState
    } // end namespace ActorsCP.Actors