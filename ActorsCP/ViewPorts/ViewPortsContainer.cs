using System;
using System.Collections.Generic;
using System.Text;
using ActorsCP.Helpers;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Контейнер вьюпортов
    /// </summary>
    public sealed class ViewPortsContainer : DisposableImplementation<ViewPortsContainer>
        {
        #region Мемберы

        /// <summary>
        /// Список внешних объектов (реализующих интерфейс IActorViewPort) которые вызвали
        /// метод BindEventsHandlers() Нужен для того, чтобы привязать объекты, созданные при вызове
        /// метода Run() Список дополняется в методе BindEventsHandlers() и освобождается в методе UnbindEventsHandlers()
        /// </summary>
        private readonly List<WeakReference> __iViewPortList = new List<WeakReference>();

        #endregion Мемберы

        #region Свойства

        /// <summary>
        /// Список вьюпортов
        /// </summary>
        public List<WeakReference> ViewPortsList
            {
            get
                {
                return __iViewPortList;
                }
            }

        #endregion Свойства

        public void DeleteWeakReference(IActorViewPort iViewPort)
            {
            WeakReferenceHelper.DeleteWeakReference(__iViewPortList, iViewPort);
            }

        public void Add(IActorViewPort iViewPort)
            {
            __iViewPortList.Add(new WeakReference(iViewPort, false));
            }

        public List<WeakReference> GetCopy()
            {
            var l = new List<WeakReference>(ViewPortsList);
            return l;
            }

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            __iViewPortList.Clear();
            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable
        }
    }