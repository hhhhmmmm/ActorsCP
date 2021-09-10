using System;
using System.Text;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    /// <summary>
    /// Основа тестов
    /// </summary>
    public class TestBase
        {
        private bool IsInitialized;

        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void BaseInit()
            {
            if (IsInitialized)
                {
                return;
                }

            IsInitialized = true;
            }

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public TestBase()
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
        } // end class TestBase
    } // end namespace ActorsCP.Unit.Test