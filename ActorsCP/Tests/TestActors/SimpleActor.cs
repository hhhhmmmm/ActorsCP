using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        /// <summary>
        /// Перегружаемая функция для выполнения некоторых действий
        /// </summary>
        /// <returns>true если объект все выполнил успешно</returns>
        protected override async Task<bool> InternalRunAsync()
            {
            //await Task.Delay(1000);
            return await base.InternalRunAsync();
            }
        } // end class SimpleActor
    } // end namespace ActorsCP.TestActors