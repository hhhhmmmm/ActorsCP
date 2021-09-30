using System;
using System.Windows.Forms;

using ActorsCP.Actors;

namespace ActorsCP.dotNET.ViewPorts.TreeViewViewPort
    {
    /// <summary>
    /// Внутренний класс для отрисовки акторов
    /// </summary>
    internal class ActorsTreeView : TreeView
        {
        private readonly ToolTip m_toolTip = new ToolTip();

        #region Конструкторы

        public ActorsTreeView()
            {
            this.MouseMove += new MouseEventHandler(ActorsTreeView_MouseMove);
            }

        #endregion Конструкторы

        /// <summary>
        /// Обработчик движения мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActorsTreeView_MouseMove(object sender, MouseEventArgs e)
            {
            // Get the node at the current mouse pointer location.
            TreeNode theNode = GetNodeAt(e.X, e.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if ((theNode != null))
                {
                // Verify that the tag property is not "null".
                if (theNode.Tag != null)
                    {
                    // Change the ToolTip only if the pointer moved to a new node.
                    if (theNode.Tag.ToString() != this.m_toolTip.GetToolTip(this))
                        {
                        this.m_toolTip.SetToolTip(this, theNode.Tag.ToString());
                        }
                    }
                else
                    {
                    this.m_toolTip.SetToolTip(this, string.Empty);
                    }
                }
            else     // Pointer is not over a node so clear the ToolTip.
                {
                this.m_toolTip.SetToolTip(this, string.Empty);
                }
            }

        /// <summary>
        /// Установить изображение для узла дерева
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="Image"></param>
        public static void SetImage(TreeNode Node, TreeViewImage Image)
            {
            //  int iIndex = (int)Image;
            //  Node.ImageIndex = iIndex;
            //   Node.SelectedImageIndex = iIndex;
            }

        /// <summary>
        /// Добавить узел типа TreeViewActorNode в дерево
        /// </summary>
        /// <param name="ActorNode">узел типа TreeViewActorNode</param>
        /// <param name="ParentTreeNode">родительский узел типа TreeViewActorNode</param>
        public void AddActorNodeToTree(TreeViewActorNode ActorNode, TreeViewActorNode ParentTreeNode)
            {
            if (ActorNode == null)
                {
                return;
                }

            ActorBase Actor;
            Actor = ActorNode.Actor;
            if (Actor == null)
                {
                return;
                }

            ActorBase ParentActor = Actor.Parent;

            TreeNodeCollection ParentNode = null;

            if (ParentActor == null)
                {
                ParentNode = Nodes;
                }
            else
                {
                if (ParentTreeNode != null)
                    {
                    ParentNode = ParentTreeNode.Nodes;
                    }
                else
                    {
                    // ParentNode = Nodes;
                    }
                }

            #region Метод добавления узла в дерево

            void ADD()
                {
                ParentNode.Add(ActorNode);
                ActorNode.AddActionsNode();
                if (ParentActor == null)
                    {
                    ActorNode.Expand();
                    }
                }

            #endregion Метод добавления узла в дерево

            if (InvokeRequired)
                {
                Invoke(new MethodInvoker(delegate
                    {
                        ADD();
                        }));
                }
            else
                {
                ADD();
                }
            }
        }
    }