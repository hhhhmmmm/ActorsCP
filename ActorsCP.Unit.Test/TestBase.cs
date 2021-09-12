using System;
using System.Diagnostics;
using System.Text;
using ActorsCP.Actors;
using ActorsCP.Helpers;
using ActorsCP.TestActors;
using NUnit.Framework;

namespace ActorsCP.Unit.Test
    {
    [TestFixture]
    /// <summary>
    /// Основа тестов
    /// </summary>
    public class TestBase : IMessageChannel
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

        private void ConfigureActor(ActorBase actor)
            {
            actor.SetIMessageChannel(this);
            }

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