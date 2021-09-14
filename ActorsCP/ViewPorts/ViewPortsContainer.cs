using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ActorsCP.Actors;
using ActorsCP.Helpers;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Контейнер вьюпортов
    /// </summary>
    public sealed class ViewPortsContainer : DisposableImplementation<ViewPortsContainer>
        {
        #region Мемберы

        private readonly ActorBase _parentActor;

        /// <summary>
        /// Список внешних объектов (реализующих интерфейс IActorViewPort) которые вызвали
        /// метод BindEventsHandlers() Нужен для того, чтобы привязать объекты, созданные при вызове
        /// метода Run() Список дополняется в методе BindEventsHandlers() и освобождается в методе UnbindEventsHandlers()
        /// </summary>
        private readonly List<WeakReference> _iViewPortList = new List<WeakReference>();

        private int _bindChildCounter;

        #endregion Мемберы

        public ViewPortsContainer(ActorBase parentActor = null)
            {
            _parentActor = parentActor;
            }

        #region Свойства

        /// <summary>
        /// Список вьюпортов
        /// </summary>
        public List<WeakReference> ViewPortsList
            {
            get
                {
                return _iViewPortList;
                }
            }

        public int BindChildCounter
            {
            get
                {
                return _bindChildCounter;
                }
            }

        public bool IsEmpty
            {
            get
                {
                return _iViewPortList.Count == 0;
                }
            }

        #endregion Свойства

        public void DeleteWeakReference(IActorViewPort iViewPort)
            {
            WeakReferenceHelper.DeleteWeakReference(_iViewPortList, iViewPort);
            }

        public void Add(IActorViewPort iViewPort)
            {
            _iViewPortList.Add(new WeakReference(iViewPort, false));
            }

        public List<WeakReference> GetCopy()
            {
            var l = new List<WeakReference>(ViewPortsList);
            return l;
            }

        public void BindEventsHandlers(ActorBase childActor)
            {
            foreach (var wr in ViewPortsList)
                {
                if (wr.IsAlive)
                    {
                    var eh = wr.Target as IActorViewPort;
                    if (eh != null)
                        {
                        childActor.BindEventsHandlers(eh);
                        }
                    } // end IsAlive
                } // end foreach

            Interlocked.Increment(ref _bindChildCounter);
            }

        public void UnbindEventsHandlers(ActorBase childActor)
            {
            foreach (var wr in ViewPortsList)
                {
                if (wr.IsAlive)
                    {
                    var eh = wr.Target as IActorViewPort;
                    if (eh != null)
                        {
                        childActor.UnbindEventsHandlers(eh);
                        }
                    } // end IsAlive
                } // end foreach

            Interlocked.Decrement(ref _bindChildCounter);
            }

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            _iViewPortList.Clear();
            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable
        }
    }