using System;
using System.Text;

namespace ActorsCP.Actors
    {
    /// <summary>
    ///
    /// </summary>
    public class ActorsQueue : ActorsSet
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorsQueue()
            {
            SetName("Очередь объектов");
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название очереди объектов</param>
        public ActorsQueue(string name) : base(name)
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
        } // end class ActorsQueue
    } // end namespace ActorsCP.Actors