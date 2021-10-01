using System;
using System.Globalization;
using System.Windows.Forms;
using ActorsCP.Actors;

namespace ActorsCP.dotNET.ViewPorts.Tree
    {
    /// <summary>
    /// Класс для описания одного узла дерева содержащего объект GuActor
    /// </summary>
    public sealed class TreeViewActorNode : TreeNode
        {
        #region Приватные мемберы

        /// <summary>
        /// Слабая ссылка на объект типа ActorBase
        /// </summary>
        private readonly WeakReference _actorWeakReference;

        /// <summary>
        /// Узел дерева 'События'
        /// </summary>
        private TreeNode _actionsTreeNode;

        #endregion Приватные мемберы

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="actor">Объект</param>
        public TreeViewActorNode(ActorBase actor)
            {
            _actorWeakReference = new WeakReference(actor);
            ActorsTreeView.SetImage(this, TreeViewImage.ActorWaiting);
            Text = actor.Name;
            }

        #endregion Конструктор

        /// <summary>
        /// Добавить подузел дерева 'События'
        /// </summary>
        public void AddActionsNode()
            {
            _actionsTreeNode = new TreeNode("События");

            // ActorsTreeView.SetImage(_actionsTreeNode, TreeViewImage.ActionNode);
            Nodes.Add(_actionsTreeNode);
            }

        /// <summary>
        /// Установить иконку
        /// </summary>
        /// <param name="image">Иконка</param>
        public void SetImage(TreeViewImage image)
            {
            if (TreeView == null)
                {
                return;
                }

            ActorsTreeView.SetImage(this, image);
            }

        #region Работа с узлом 'События'

        /// <summary>
        /// Добавить запись в узел 'События'
        /// </summary>
        /// <param name="text">Текст</param>
        public void AddAction(string text)
            {
            AddAction(text, TreeViewImage.ActionSystemNeutral);
            }

        /// <summary>
        /// Добавить запись в узел 'События'
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="image">Иконка</param>
        public void AddAction(string text, TreeViewImage image)
            {
            //if (_actionsTreeNode?.TreeView == null)
            //    {
            //    return;
            //    }

            if (_actionsTreeNode.TreeView.IsDisposed)
                {
                return;
                }

            DateTime now = DateTime.Now;
            var resultingText = string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}.{3:000} - {4}", now.Hour, now.Minute, now.Second, now.Millisecond, text);

            var ac = new TreeNode(resultingText);

            _actionsTreeNode.Nodes.Add(ac);
            ActorsTreeView.SetImage(ac, image);
            }

        /// <summary>
        ///
        /// </summary>
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
                ActorBase a = _actorWeakReference.Target as ActorBase;
                return a;
                }
            }

        #endregion Свойства
        }
    }