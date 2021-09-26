using System;
using System.Globalization;
using System.Windows.Forms;
using ActorsCP.Actors;

namespace ActorsCP.dotNET.ViewPorts.TreeViewViewPort
    {
    /// <summary>
    /// Класс для описания одного узла дерева содержащего объект GuActor
    /// </summary>
    public sealed class TreeViewActorNode : TreeNode
        {
        /// <summary>
        /// Слабая ссылка на объект типа GuActor
        /// </summary>
        private readonly WeakReference m_ActorWR;

        /// <summary>
        /// Узел дерева 'События'
        /// </summary>
        private TreeNode m_ActionsNode;

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="oActor">Объект</param>
        public TreeViewActorNode(ActorBase oActor)
            {
            m_ActorWR = new WeakReference(oActor);

            //if (string.IsNullOrEmpty(Actor.Description))
            //    {
            //    this.Text = Actor.Name;
            //    this.ToolTipText = Actor.Description;
            //    }
            //else
            //    {
            //    this.Text = string.Format("{0} ({1})", Actor.Name, Actor.Description.Trim());
            //    }

            FastTreeView.SetImage(this, TreeViewImage.ActorWaiting);
            }

        #endregion Конструктор

        /// <summary>
        /// Добавить подузел дерева 'События'
        /// </summary>
        public void AddActionsNode()
            {
            m_ActionsNode = new TreeNode("События");

            ActorsTreeView.SetImage(m_ActionsNode, TreeViewImage.ActionNode);

            Nodes.Add(m_ActionsNode);
            }

        /// <summary>
        /// Установить иконку
        /// </summary>
        /// <param name="Image">Иконка</param>
        public void SetImage(TreeViewImage Image)
            {
            if (TreeView == null)
                {
                return;
                }

            ActorsTreeView.SetImage(this, Image);
            }

        #region Работа с узлом 'События'

        /// <summary>
        /// Добавить запись в узел 'События'
        /// </summary>
        /// <param name="Text">Текст</param>
        public void AddAction(string Text)
            {
            AddAction(Text, TreeViewImage.ActionSystemNeutral);
            }

        /// <summary>
        /// Добавить запись в узел 'События'
        /// </summary>
        /// <param name="Text">Текст</param>
        /// <param name="Image">Иконка</param>
        public void AddAction(string Text, TreeViewImage Image)
            {
            if (m_ActionsNode == null || m_ActionsNode.TreeView == null)
                {
                return;
                }

            if (m_ActionsNode.TreeView.IsDisposed)
                {
                return;
                }

            DateTime now = DateTime.Now;
            string ResultingText = string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}.{3:000} - {4}", now.Hour, now.Minute, now.Second, now.Millisecond, Text);

            TreeNode ac = new TreeNode(ResultingText);

            m_ActionsNode.Nodes.Add(ac);
            ActorsTreeView.SetImage(ac, Image);
            }

        public void TryExpandAll()
            {
            if (TreeView == null)
                {
                return;
                }

            ExpandAll();
            }

        #endregion Работа с узлом 'События'

        #region Свойства

        /// <summary>
        /// Объект
        /// </summary>
        public ActorBase Actor
            {
            get
                {
                ActorBase a = m_ActorWR.Target as ActorBase;
                return a;
                }
            }

        #endregion Свойства
        }
    }