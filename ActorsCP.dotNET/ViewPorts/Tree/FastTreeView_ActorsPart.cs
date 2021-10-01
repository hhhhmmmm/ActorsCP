using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ActorsCP.Actors;

namespace ActorsCP.dotNET.ViewPorts.Tree
    {
    /// <summary>
    ///
    /// </summary>
    partial class FastTreeView
        {
        private readonly ToolTip _toolTip = new ToolTip();

        /// <summary>
        /// Обработчик движения мыши, показывает ToolTip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void ActorsTreeView_MouseMove(object sender, MouseEventArgs e)
        //    {
        //    // Get the node at the current mouse pointer location.
        //    TreeNode theNode = GetNodeAt(e.X, e.Y);

        //    // Set a ToolTip only if the mouse pointer is actually paused on a node.
        //    if ((theNode != null))
        //        {
        //        // Verify that the tag property is not "null".
        //        if (theNode.Tag != null)
        //            {
        //            // Change the ToolTip only if the pointer moved to a new node.
        //            if (theNode.Tag.ToString() != _toolTip.GetToolTip(this))
        //                {
        //                _toolTip.SetToolTip(this, theNode.Tag.ToString());
        //                }
        //            }
        //        else
        //            {
        //            _toolTip.SetToolTip(this, string.Empty);
        //            }
        //        }
        //    else     // Pointer is not over a node so clear the ToolTip.
        //        {
        //        _toolTip.SetToolTip(this, string.Empty);
        //        }
        //    }

        private static void ADD(TreeNodeCollection parentNode, TreeViewActorNode actorNode, ActorBase parentActor)
            {
            parentNode.Add(actorNode);
            actorNode.AddActionsNode();
            if (parentActor == null)
                {
                actorNode.Expand();
                }
            }

        /// <summary>
        /// Добавить узел типа TreeViewActorNode в дерево
        /// </summary>
        /// <param name="treeViewActorNode">узел типа TreeViewActorNode</param>
        /// <param name="parentTreeViewActorNode">родительский узел типа TreeViewActorNode</param>
        public void AddActorNodeToTree(TreeViewActorNode treeViewActorNode, TreeViewActorNode parentTreeViewActorNode)
            {
            if (treeViewActorNode == null)
                {
                throw new ArgumentNullException($"{nameof(treeViewActorNode)} не может быть null");
                }

            var actor = treeViewActorNode.Actor;
            if (actor == null)
                {
                throw new ArgumentNullException($"{nameof(actor)} не может быть null");
                }

            ActorBase parentActor = actor.Parent;

            TreeNodeCollection ParentNode = null;

            if (parentActor == null)
                {
                ParentNode = Nodes;
                }
            else
                {
                if (parentTreeViewActorNode != null)
                    {
                    ParentNode = parentTreeViewActorNode.Nodes;
                    }
                else
                    {
                    ParentNode = Nodes;
                    }
                }

            if (InvokeRequired)
                {
                Invoke(new MethodInvoker(delegate
                    {
                        ADD(ParentNode, treeViewActorNode, parentActor);
                        }));
                }
            else
                {
                ADD(ParentNode, treeViewActorNode, parentActor);
                }
            }
        } // end class FastTreeView
    } // end namespace ActorsCP.dotNET.ViewPorts.Tree