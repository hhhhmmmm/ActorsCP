using System;
using System.Diagnostics;
using System.Text;
using ActorsCP.Actors;
using ActorsCP.Helpers;
using ActorsCP.TestActors;
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
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        ///
        /// </summary>
        /// <param name="actor"></param>
        private void ConfigureActor(ActorBase actor)
            {
            actor.SetIMessageChannel(this);
            }

        #region Генераторы акторов

        /// <summary>
        /// Новый пустой актор
        /// </summary>
        public ActorBase NewPendingActor
            {
            get
                {
                var actor = new SimpleActor();
                ConfigureActor(actor);
                return actor;
                }
            }

        /// <summary>
        /// Новый актор ExceptionActor
        /// </summary>
        public ExceptionActor NewExceptionActor
            {
            get
                {
                var actor = new ExceptionActor();
                ConfigureActor(actor);
                return actor;
                }
            }

        #endregion Генераторы акторов

        #endregion Свойства
        } // end class TestBase
    } // end namespace ActorsCP.Unit.Test