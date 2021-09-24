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

        /// <summary>
        /// Констанстный пустой актор
        /// </summary>
        public static EmptyActor Value
            {
            get;
            } = new EmptyActor();
        } // end class EmptyActor
    } // end namespace ActorsCP.UtilityActors