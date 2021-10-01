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

            UpdateTitleByActorState();
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
        /// Обновить состояние в дереве по его текущему состоянию
        /// </summary>
        public void UpdateTitleByActorState()
            {
            ActorBase actor = null;
            if (_actorWeakReference.IsAlive)
                {
                actor = _actorWeakReference.Target as ActorBase;
                }
            if (actor == null)
                {
                return;
                }

            switch (actor.State)
                {
                case ActorState.Pending:
                    {
                    Text = actor.Name + " - ожидание";
                    SetImage(this, TreeViewImageIndex.Actor_Pending);
                    break;
                    }
                case ActorState.Started:
                    {
                    Text = actor.Name + " - запущен";
                    SetImage(this, TreeViewImageIndex.Actor_Started);
                    break;
                    }
                case ActorState.Running:
                    {
                    Text = actor.Name + " - работает";
                    SetImage(this, TreeViewImageIndex.Actor_Running);
                    break;
                    }
                case ActorState.Stopped:
                    {
                    Text = actor.Name + " - остановлен";
                    SetImage(this, TreeViewImageIndex.Actor_Stopped);
                    break;
                    }
                case ActorState.Terminated:
                    {
                    if (actor.AnErrorOccurred)
                        {
                        Text = actor.Name + " - завершен с ошибкой";
                        SetImage(this, TreeViewImageIndex.Actor_Terminated_Failure);
                        }
                    else
                        {
                        Text = actor.Name + " - завершен успешно";
                        SetImage(this, TreeViewImageIndex.Actor_Terminated_OK);
                        }
                    break;
                    }
                }
            }

        /// <summary>
        /// Создать необходимые доп. узлы после добавления основного узла в дерево
        /// </summary>
        public void Propagate()
            {
            AddActionsNode();
            }

        /// <summary>
        /// Добавить подузел дерева - 'События'
        /// </summary>
        private void AddActionsNode()
            {
            _actionsTreeNode = new TreeNode("События");
            SetImage(_actionsTreeNode, TreeViewImageIndex.ActionNode);
            Nodes.Add(_actionsTreeNode);
            }

        /// <summary>
        /// Установить изображение для узла дерева
        /// </summary>
        /// <param name="node">Узел</param>
        /// <param name="imageIndex">Индекс картинки в списке</param>
        private void SetImage(TreeNode node, TreeViewImageIndex imageIndex)
            {
            node.ImageIndex = (int)imageIndex;
            node.SelectedImageIndex = (int)imageIndex;
            }

        #region Работа с узлом 'События'

        /// <summary>
        /// Добавить запись в узел 'События'
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="image">Иконка</param>
        private void AddActionToTree(string text, TreeViewImageIndex image)
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

            SetImage(ac, image);
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

            if (treeNode == null)
                {
                return;
                }

            var str = actorEventArgs.EventDateAsString + " ";

            switch (actorEventArgs)
                {
                case ActorExceptionEventArgs exception:
                    {
                    str = str + "Исключение: " + exception.MessageText;
                    AddActionToTree(str, TreeViewImageIndex.ActionException);
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
                            AddActionToTree(str, TreeViewImageIndex.ActionDebug);

                            break;
                            }
                        case ActorActionEventType.Neutral:
                            {
                            str = str + " " + action.MessageText;
                            AddActionToTree(str, TreeViewImageIndex.ActionNeutral);
                            break;
                            }
                        case ActorActionEventType.Warning:
                            {
                            str = str + "Предупреждение: " + action.MessageText;
                            AddActionToTree(str, TreeViewImageIndex.ActionWarning);
                            break;
                            }
                        case ActorActionEventType.Error:
                            {
                            str = str + "Ошибка: " + action.MessageText;
                            AddActionToTree(str, TreeViewImageIndex.ActionError);
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
            }

        #endregion Работа с узлом 'События'
        }
    }