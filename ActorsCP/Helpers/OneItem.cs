using System;
using System.Text;

namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Один элемент, для комбобокса и пр.
    /// </summary>
    public class OneItem
        {
        private readonly bool _includeValueInName;

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="value">Значение</param>
        /// <param name="includeValueInName">Включать значение в ToString()</param>
        public OneItem(string name, string value, bool includeValueInName = false)
            {
            if (name == null)
                {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} не может быть null");
                }
            if (value == null)
                {
                throw new ArgumentNullException(nameof(value), $"{nameof(value)} не может быть null");
                }

            Name = name;
            Value = value;
            _includeValueInName = includeValueInName;
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        public string Name
            {
            get;
            }

        /// <summary>
        /// Значение
        /// </summary>
        public string Value
            {
            get;
            }

        #endregion Свойства

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            {
            if (!_includeValueInName)
                {
                return Name;
                }
            return Name + "(" + Value + ")";
            }
        } // end class OneItem
    } // end namespace ActorsCP.Helpers