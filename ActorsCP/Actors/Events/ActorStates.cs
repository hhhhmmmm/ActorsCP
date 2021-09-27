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
        public static ActorStateChangedEventArgs ActorStateChangedPending
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Pending);

        /// <summary>
        /// Переведен в состояние работы
        /// </summary>
        public static ActorStateChangedEventArgs ActorStateChangedStarted
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Started);

        /// <summary>
        /// Остановлен
        /// </summary>
        public static ActorStateChangedEventArgs ActorStateChangedRunning
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Running);

        /// <summary>
        /// Работает
        /// </summary>
        public static ActorStateChangedEventArgs ActorStateChangedStopped
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Stopped);

        /// <summary>
        /// Завершен навсегда
        /// </summary>
        public static ActorStateChangedEventArgs ActorStateChangedTerminated
            {
            get;
            } = new ActorStateChangedEventArgs(ActorState.Terminated);
        } // end class ActorStates
    } // end namespace ActorsCP.Actors.Events