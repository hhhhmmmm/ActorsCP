using System;
using System.Globalization;
using System.Windows.Forms;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;

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
            if (actor == null)
                {
                throw new ArgumentNullException($"{nameof(actor)} не может быть null");
                }
            _actorWeakReference = new WeakReference(actor);
            SetImage(TreeViewImage.ActorWaiting);
            Text = actor.Name;
            Tag = Text;
            }

        #endregion Конструктор

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

        /// <summary>
        /// Добавить подузел дерева - 'События'
        /// </summary>
        public void AddActionsNode()
            {
            _actionsTreeNode = new TreeNode("События");
            SetImage(TreeViewImage.ActionNode);
            Nodes.Add(_actionsTreeNode);
            }

        /// <summary>
        /// Установить изображение для узла дерева
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="Image"></param>
        public static void SetImage(TreeViewImage image)
            {
            //  int iIndex = (int)Image;
            //  Node.ImageIndex = iIndex;
            //   Node.SelectedImageIndex = iIndex;
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
            if (_actionsTreeNode.TreeView.IsDisposed)
                {
                return;
                }

            var ac = new TreeNode(text);

            if (_actionsTreeNode.TreeView.InvokeRequired)
                {
                _actionsTreeNode.TreeView.Invoke(new MethodInvoker(delegate
                    {
                        AddNode(_actionsTreeNode, ac);
                        }));
                }
            else
                {
                AddNode(_actionsTreeNode, ac);
                }

            // FastTreeView.SetImage(ac, image);
            }

        private static void AddNode(TreeNode actionsTreeNode, TreeNode newTreeNode)
            {
            actionsTreeNode.Nodes.Add(newTreeNode);
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="actorEventArgs"></param>
        public void AddAction(ActorBase actor, ActorEventArgs actorEventArgs)
            {
            TreeViewActorNode treeNode = actor.TreeViewGetNode();

            string str;

            str = actorEventArgs.EventDateAsString + " ";
            switch (actorEventArgs)
                {
                case ActorExceptionEventArgs exception:
                    {
                    str = str + "Исключение: " + exception.Exception.ToString();
                    break;
                    }
                case ActorActionEventArgs action:
                    {
                    #region Тип события

                    switch (action.ActionEventType)
                        {
                        case ActorActionEventType.Debug:
                            {
                            str = str + "Отладка: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Error:
                            {
                            str = str + "Ошибка: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Exception:
                            {
                            str = str + "Исключение: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Neutral:
                            {
                            str = str + " " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Warning:
                            {
                            str = str + "Предупреждение: " + action.MessageText;
                            break;
                            }
                        }

                    #endregion Тип события

                    break;
                    }
                default:
                    {
                    throw new Exception($"Непонятный тип объекта {actorEventArgs}");
                    }
                }

            AddAction(str);
            }

        #endregion Работа с узлом 'События'
        }
    }