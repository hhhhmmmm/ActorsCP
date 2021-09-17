using System;
using System.Text;

namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Вспомогательный класс для построения интерфейса IEquatable
    /// </summary>
    public static class IEquatableHelper
        {
        /// <summary>
        /// Оператор равенства ==
        /// </summary>
        /// <typeparam name="T">тип данных</typeparam>
        /// <param name="m1">Первый T</param>
        /// <param name="m2">Второй T</param>
        /// <returns>true если равны</returns>
        public static bool OperatorEquals<T>(T m1, T m2)
            {
            if (object.ReferenceEquals(m1, m2))
                return true;
            if (object.ReferenceEquals(m1, null))
                return false;
            if (object.ReferenceEquals(m2, null))
                return false;
            return m1.Equals(m2);
            }

        /// <summary>
        /// Оператор неравенства !=
        /// </summary>
        /// <typeparam name="T">тип данных</typeparam>
        /// <param name="m1">Первый T</param>
        /// <param name="m2">Второй T</param>
        /// <returns>true если не равны</returns>
        public static bool OperatorNotEquals<T>(T m1, T m2)
            {
            if (object.ReferenceEquals(m1, m2))
                return false;
            if (object.ReferenceEquals(m1, null))
                return true;
            if (object.ReferenceEquals(m2, null))
                return true;

            return !m1.Equals(m2);
            }
        } // end class IEquatableHelper
    } // end namespace ActorsCP.Helpers