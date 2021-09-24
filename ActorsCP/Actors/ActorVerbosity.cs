namespace ActorsCP.Actors
    {
    /// <summary>
    /// Многословность объекта
    /// </summary>
    public enum ActorVerbosity
        {
        /// <summary>
        /// Работаем молча
        /// </summary>
        Off = 0,

        /// <summary>
        /// Запускается...
        /// </summary>
        Starting = 0,

        /// <summary>
        /// Запущен
        /// </summary>
        Started = 1,

        /// <summary>
        /// Останавливается
        /// </summary>
        Stopping = 2,

        /// <summary>
        /// Остановлен
        /// </summary>
        Stopped = 3,

        ///// <summary>
        ///// Завершен навсегда
        ///// </summary>
        //Terminated = 4
        } // end enum ActorState
    } // end namespace ActorsCP.Actors