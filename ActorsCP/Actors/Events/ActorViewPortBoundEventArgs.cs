using System;
using System.Text;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    /// Событие - объект привязан или отвязан
    /// </summary>
    public sealed class ActorViewPortBoundEventArgs : ActorViewPortEventArgs
        {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="bound">Если true то вьюпорт привязан. Если false то вьюпорт отвязан</param>
        public ActorViewPortBoundEventArgs(bool bound)
            {
            Bound = bound;
            }

        /// <summary>
        /// Если true то вьюпорт привязан
        /// Если false то вьюпорт отвязан
        /// </summary>
        public bool Bound
            {
            get;
            }

        #region Статические объекты

        /// <summary>
        /// Статический объект
        /// </summary>
        public readonly static ActorViewPortBoundEventArgs BoundInstance = new ActorViewPortBoundEventArgs(true);

        /// <summary>
        ///
        /// </summary>
        public readonly static ActorViewPortBoundEventArgs UnboundInstance = new ActorViewPortBoundEventArgs(false);

        #endregion Статические объекты
        }
    }