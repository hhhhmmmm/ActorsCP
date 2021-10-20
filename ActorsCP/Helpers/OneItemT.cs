using System;
using System.Text;

namespace ActorsCP.Helpers
    {
    /// <summary>
    ///
    /// </summary>
    public class OneItemT<Typename>
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="value">Значение</param>
        public OneItemT(string name, Typename value)
            {
            Name = name;
            Value = value;
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        ///
        /// </summary>
        public string Name
            {
            get;
            private set;
            }

        /// <summary>
        ///
        /// </summary>
        public Typename Value
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
            return Name;
            }
        }
    }