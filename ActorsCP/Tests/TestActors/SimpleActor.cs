using System;
using System.Collections.Generic;
using System.Text;

using ActorsCP.Actors;

namespace ActorsCP.Tests.TestActors
    {
    /// <summary>
    ///
    /// </summary>
    public class SimpleActor : TestActorBase
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parameter"></param>
        public SimpleActor()
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">name</param>
        public SimpleActor(string name) : base(name)
            {
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        ///
        /// </summary>
        // public string Property
        //     {
        //     get;
        //     set;
        //     }

        #endregion Свойства

        //
        } // end class SimpleActor
    } // end namespace ActorsCP.TestActors