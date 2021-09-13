using System;
using System.Text;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    /// Объекты событий
    /// </summary>
    public static class ActorStates
        {
        /// <summary>
        /// Ожидание запуска - начальное состояние
        /// </summary>
        public static ActorStateChangedEventArgs Pending
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Pending);

        /// <summary>
        /// Переведен в состояние работы
        /// </summary>
        public static ActorStateChangedEventArgs Started
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Started);

        /// <summary>
        /// Работает
        /// </summary>
        public static ActorStateChangedEventArgs Stopped
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Stopped);

        /// <summary>
        /// Остановлен
        /// </summary>
        public static ActorStateChangedEventArgs Running
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Running);

        /// <summary>
        /// Завершен навсегда
        /// </summary>
        public static ActorStateChangedEventArgs Terminated
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Terminated);
        } // end class ActorStates
    } // end namespace ActorsCP.Actors.Events