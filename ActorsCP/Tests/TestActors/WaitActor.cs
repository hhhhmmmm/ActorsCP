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
    public class WaitActor : TestActorBase
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public WaitActor()
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">name</param>
        public WaitActor(string name) : base(name)
            {
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        ///
        /// </summary>
        public int Interval
            {
            get;
            set;
            } = 1000;

        #endregion Свойства

        /// <summary>
        /// Перегружаемая функция для выполнения некоторых действий
        /// </summary>
        /// <returns>true если объект все выполнил успешно</returns>
        protected override async Task<bool> InternalRunAsync()
            {
            OnActorActionDebug("что-то отладочное");
            OnActorAction("Обычный текст");
            OnActorActionWarning("о чем-то предупредить");
            OnActorActionError("сообщить об ошибке");
            var exception = new Exception("Исключение");
            OnActorThrownAnException(exception);
            await Task.Delay(Interval);
            return await base.InternalRunAsync();
            }
        } // end class WaitActor
    } // end namespace ActorsCP.Tests.TestActors