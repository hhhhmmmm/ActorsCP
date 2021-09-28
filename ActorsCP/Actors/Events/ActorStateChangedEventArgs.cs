using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    ///
    /// </summary>
    [DebuggerDisplay("AEA = {AEA}, State = {State}")]
    public class ActorStateChangedEventArgs : ActorEventArgs
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="state">Состояние объекта</param>
        public ActorStateChangedEventArgs(ActorState state)
            {
            State = state;
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Состояние объекта
        /// </summary>
        public ActorState State
            {
            get;
            private set;
            }

        #endregion Свойства

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            {
            return "ActorStateChangedEventArgs: State = " + State;
            }
        } // end class ActorStateChangedEventArgs
    } // end namespace ActorsCP.Actors.Events