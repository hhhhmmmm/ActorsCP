using System;
using System.Diagnostics;
using System.Text;

using ActorsCP.Actors;
using ActorsCP.Helpers;
using ActorsCP.Tests.TestActors;

using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    /// <summary>
    /// Основа тестов
    /// </summary>
    [TestFixture]
    public class TestBase : IMessageChannel
        {
        private bool _isInitialized;
        private TestActorsBuilder _testActorsBuilder;

        /// <summary>
        /// Инициализация
        /// </summary>
        [SetUp]
        public void BaseInit()
            {
            if (_isInitialized)
                {
                return;
                }

            _isInitialized = true;
            }

        #region IMessageChannel

        public void RaiseDebug(string debugText)
            {
            Debug.WriteLine($"RaiseDebug: {debugText}");
            }

        public void RaiseMessage(string messageText)
            {
            Debug.WriteLine($"RaiseMessage: {messageText}");
            }

        public void RaiseWarning(string warningText)
            {
            Debug.WriteLine($"RaiseWarning: {warningText}");
            }

        public void RaiseError(string errorText)
            {
            Debug.WriteLine($"RaiseError: {errorText}");
            }

        public void RaiseException(Exception exception)
            {
            Debug.WriteLine($"RaiseException: {exception}");
            }

        #endregion IMessageChannel

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public TestBase()
            {
            _testActorsBuilder = new TestActorsBuilder(this);
            }

        #endregion Конструкторы

        #region Свойства

        public TestActorsBuilder TestActorsBuilder
            {
            get
                {
                return _testActorsBuilder;
                }
            }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actor"></param>
        private void ConfigureActor(ActorBase actor)
            {
            actor.SetIMessageChannel(this);
            }

        #region Генераторы объекта

        /// <summary>
        /// Новый пустой объект
        /// </summary>
        public ActorBase NewPendingActor
            {
            get
                {
                return TestActorsBuilder.NewSimpleActor;
                }
            }

        /// <summary>
        /// Новый объект ExceptionActor
        /// </summary>
        public ExceptionActor NewExceptionActor
            {
            get
                {
                return TestActorsBuilder.NewExceptionActor;
                }
            }

        #endregion Генераторы объекта

        #endregion Свойства
        } // end class TestBase
    } // end namespace ActorsCP.Unit.Test