using System;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Многословность объекта - о каких событиях он будет сообщать
    /// </summary>
    [Flags]
    public enum ActorVerbosity
        {
        /// <summary>
        /// Работаем молча
        /// </summary>
        Off = 0,

        /// <summary>
        /// Запуск...
        /// </summary>
        Starting = 1,

        /// <summary>
        /// Запущен
        /// </summary>
        Started = 2,

        /// <summary>
        /// Выполнение
        /// </summary>
        Running = 4,

        /// <summary>
        /// Останавливается
        /// </summary>
        Stopping = 8,

        /// <summary>
        /// Остановлен
        /// </summary>
        Stopped = 16,

        ///// <summary>
        ///// Завершен навсегда
        ///// </summary>
        //Terminated = 4
        } // end enum ActorState
    } // end namespace ActorsCP.Actors