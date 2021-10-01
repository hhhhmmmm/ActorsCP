using System;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;

namespace ActorsCP.dotNET.ViewPorts.Tree
    {
    /// <summary>
    /// Расширения ActorBase
    /// </summary>
    public static class ActorBaseExtensions
        {
        /// <summary>
        /// Ключ в словаре объекта для хранения TreeViewActorNode
        /// </summary>
        private const string TREEVIEW_POINTER_TO_ACTOR_NODE = "TreeView_PointerToActorNode";

        /// <summary>
        /// Добавить узел в дерево для объекта
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="treeView">Дерево</param>
        internal static TreeViewActorNode TreeViewBind(this ActorBase actor, FastTreeView treeView)
            {
            var actorNode = new TreeViewActorNode(actor);

            var bres = actor.ExternalObjects.TryAdd(TREEVIEW_POINTER_TO_ACTOR_NODE, new WeakReference(actorNode));
            if (!bres)
                {
                throw new Exception("Что-то пошло не так");
                }

            TreeViewActorNode parentTreeNode = null;

            if (actor.Parent != null)
                {
                parentTreeNode = actor.Parent.TreeViewGetNode();
                }

            treeView.AddActorNodeToTree(actorNode, parentTreeNode);
            return actorNode;
            }

        /// <summary>
        /// Извлечь из объекта структуру TreeViewActorNode указывающую на узел дерева
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <returns>Структура TreeViewActorNode</returns>
        internal static TreeViewActorNode TreeViewGetNode(this ActorBase actor)
            {
            if (actor.ExternalObjects.TryGetValue(TREEVIEW_POINTER_TO_ACTOR_NODE, out WeakReference ww))
                {
                if (ww?.IsAlive == true)
                    {
                    TreeViewActorNode treeNode = ww.Target as TreeViewActorNode;
                    return treeNode;
                    }
                }
            return null;
            }

        /// <summary>
        /// Отвязать актора от дерева
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="treeView">Дерево</param>
        internal static void TreeViewUnbind(this ActorBase actor, FastTreeView treeView)
            {
            WeakReference wr = null;
            actor.ExternalObjects?.TryRemove(TREEVIEW_POINTER_TO_ACTOR_NODE, out wr);
            if (wr != null)
                {
                wr.Target = null;
                }
            }

        /// <summary>
        /// добавить действие объекта в дерево
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="actorEventArgs">Действие</param>
        internal static void TreeViewAddAction(this ActorBase actor, ActorEventArgs actorEventArgs)
            {
            TreeViewActorNode treeNode = actor.TreeViewGetNode();
            treeNode?.AddAction(actor, actorEventArgs);
            }

        /// <summary>
        /// Изменилось состояние набора
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="actorEventArgs"></param>
        internal static void TreeViewProcessStateChanged(this ActorBase actor, ActorEventArgs actorEventArgs)
            {
            //string str;

            //str = actorEventArgs.EventDateAsString + " ";

            //switch (actorEventArgs)
            //    {
            //    case ActorSetCountChangedEventArgs c:
            //        {
            //        str = str + " " + $"W: {c.WaitingCount} R: {c.RunningCount} C: {c.CompletedCount} + T {c.TotalCount}";
            //        break;
            //        }
            //    case ActorStateChangedEventArgs e:
            //        {
            //        str = str + " " + e.State;
            //        break;
            //        }
            //    default:
            //        {
            //        throw new Exception($"Непонятный тип объекта {actorEventArgs}");
            //        }
            //    }
            }
        } // end class ActorExtensions
    } // end namespace ActorsCP.dotNET.ViewPorts.Tree