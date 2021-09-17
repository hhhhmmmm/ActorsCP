using System;
using System.Collections.Generic;
using System.Text;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    ///
    /// </summary>
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

        //
        } // end class ActorStateChangedEventArgs
    } // end namespace ActorsCP.Actors.Events