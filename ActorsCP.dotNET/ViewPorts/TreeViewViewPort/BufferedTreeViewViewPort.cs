using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActorsCP.ViewPorts;

namespace ActorsCP.dotNET.ViewPorts.TreeViewViewPort
    {
    public class BufferedTreeViewViewPort : QueueBufferedActorViewPortBase
        {
        /// <summary>
        /// Дерево
        /// </summary>
        private FastTreeView m_ActorsTreeView;

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public BufferedTreeViewViewPort(Actors.ActorBase Actor, string Caption) //: base(Actor, Caption)
            {
            //  this.Name = nameof(TreeViewViewPort);
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        public BufferedTreeViewViewPort(Actors.ActorBase Actor)
            : this(Actor, null)
            {
            }

        #endregion Конструкторы

        #region Перегружаемые методы

        /// <summary>
        /// Обработать как ActorEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorEventArgs(ViewPortItem viewPortItem)
            {
            }

        /// <summary>
        /// Обработать как ActorStateChangedEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorStateChangedEventArgs(ViewPortItem viewPortItem)
            {
            }

        #endregion Перегружаемые методы
        }
    }