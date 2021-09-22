using System;
using System.Text;

using ActorsCP.Actors;

namespace ActorsCP.UtilityActors
    {
    /// <summary>
    /// Пустой актор - заглушка
    /// </summary>
    public sealed class EmptyActor : ActorBase
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public EmptyActor() : base("Заглушка")
            {
            }

        #endregion Конструкторы

        //
        } // end class EmptyActor
    } // end namespace ActorsCP.UtilityActors