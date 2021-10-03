using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;
using ActorsCP.ViewPorts;

namespace ActorsCP.dotNET.ViewPorts.Tree
    {
    /// <summary>
    /// Текстовый вьюпорт
    /// </summary>
    public sealed class TreeViewPort : FormsViewPortBase
        {
        #region Приватные мемберы

        /// <summary>
        ///
        /// </summary>
        private FastTreeView _control;

        #endregion Приватные мемберы

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="caption">Заголовок</param>
        /// <param name="icon">Иконка окна</param>
        public TreeViewPort(ActorBase actor, string caption, Icon icon = null) : base(actor, caption, icon)
            {
            }

        #endregion Конструкторы

        #region Перегружаемые методы

        /// <summary>
        /// Создать дочерний элемент управления
        /// </summary>
        /// <param name="rectangle">Элемент - образец</param>
        protected override Control InternalCreateChildControl(Rectangle rectangle)
            {
            var treeImageList = TreeViewImageList.GetInstance();

            _control = new FastTreeView()
                {
                Location = new Point(rectangle.X, rectangle.Y),
                Name = "FastTreeView1",
                Size = new Size(rectangle.Width, rectangle.Height),
                TabIndex = 0,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                Scrollable = true,
                ImageIndex = 0,
                ImageList = treeImageList.ImageList,
                SelectedImageIndex = 0,
                ShowNodeToolTips = true,
                HideSelection = false,
                BorderStyle = BorderStyle.Fixed3D,
                };

            return _control;
            }

        /// <summary>
        /// Обработать как ActorViewPortBoundEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorViewPortBoundEventArgs(ViewPortItem viewPortItem)
            {
            var actorEventArgs = viewPortItem.ActorEventArgs as ActorViewPortBoundEventArgs;
            var actor = viewPortItem.Sender;

            if (actor == null)
                {
                throw new ArgumentNullException($"{nameof(actor)} не может быть null");
                }

            if (actorEventArgs == null)
                {
                throw new ArgumentNullException($"{nameof(actorEventArgs)} не может быть null");
                }

            if (actorEventArgs.Bound)
                {
                actor.TreeViewBind(_control);
                }
            else
                {
                actor.TreeViewUnbind(_control);
                }
            }

        /// <summary>
        /// Обработать как ActorEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorEventArgs(ViewPortItem viewPortItem)
            {
            var actorEventArgs = viewPortItem.ActorEventArgs;
            var actor = viewPortItem.Sender;
            actor.TreeViewAddAction(actorEventArgs);
            }

        /// <summary>
        /// Обработать как ActorStateChangedEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorStateChangedEventArgs(ViewPortItem viewPortItem)
            {
            var actorEventArgs = viewPortItem.ActorEventArgs;
            var actor = viewPortItem.Sender;
            actor.TreeViewProcessStateChanged(actorEventArgs);
            }

        #endregion Перегружаемые методы
        }
    }