using System;
using System.Text;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// элемент описывающий событие
    /// </summary>
    public sealed class ViewPortItem
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sender">Объект-отправитель</param>
        /// <param name="actorEventArgs">Событие</param>
        public ViewPortItem(ActorBase sender, ActorEventArgs actorEventArgs)
            {
            Sender = sender;
            ActorEventArgs = actorEventArgs;
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Объект-отправитель
        /// </summary>
        public ActorBase Sender
            {
            get;
            private set;
            }

        /// <summary>
        /// Событие
        /// </summary>
        public ActorEventArgs ActorEventArgs
            {
            get;
            private set;
            }

        #endregion Свойства

        //
        } // end class ViewPortItem
    } // end namespace ActorsCP.ViewPorts