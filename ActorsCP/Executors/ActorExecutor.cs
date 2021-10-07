using System;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.ViewPorts;

namespace ActorsCP.Executors
    {
    /// <summary>
    /// Базовый класс для исполнителей Позволяет выполнять объекты в фоновом потоке по умолчанию
    /// </summary>
    public class ActorExecutor : ActorBase
        {
        #region Свойства

        /// <summary>
        /// Исполняемый объект
        /// </summary>
        public ActorBase Actor
            {
            get;
            private set;
            }

        /// <summary>
        /// Вьюпорт
        /// </summary>
        public IActorViewPort ViewPort
            {
            get;
            private set;
            }

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название исполнителя</param>
        /// <param name="actor">Исполняемый объект</param>
        /// <param name="iViewPort">Вьюпорт</param>
        public ActorExecutor(string name, ActorBase actor, IActorViewPort iViewPort) : base(name)
            {
            if (actor == null)
                {
                throw new ArgumentNullException(nameof(actor), $"{nameof(actor)} не может быть null");
                }

            if (iViewPort == null)
                {
                throw new ArgumentNullException(nameof(iViewPort), $"{nameof(iViewPort)} не может быть null");
                }

            Actor = actor;
            SetViewPort(iViewPort);
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="actor">Исполняемый объект</param>
        /// <param name="iViewPort">Вьюпорт</param>
        public ActorExecutor(ActorBase actor, IActorViewPort iViewPort) : this("Исполнитель", actor, iViewPort)
            {
            }

        #endregion Конструкторы

        /// <summary>
        /// Установить/подключить внешний вьюпорт
        /// </summary>
        /// <param name="iViewPort"></param>
        public void SetViewPort(IActorViewPort iViewPort)
            {
            ViewPort = iViewPort;
            }

        #region Перегруженные методы

        /// <summary>
        /// Старт
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override async Task<bool> InternalStartAsync()
            {
            bool bres = await base.InternalStartAsync();
            if (!bres)
                {
                return false;
                }
            Actor.SetParent(this);
            Actor.BindViewPort(ViewPort);
            Actor.SetCancellationToken(CancellationTokenSource);
            return true;
            }

        /// <summary>
        /// Запуск
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override async Task<bool> InternalRunAsync()
            {
            bool bres = await Actor.RunAsync();
            return bres;
            }

        /// <summary>
        /// Остановка
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected override async Task<bool> InternalStopAsync()
            {
            Actor.UnbindViewPort(ViewPort);
            return await base.InternalStopAsync();
            }

        /// <summary>
        /// Отменяет выполнение производного объекта
        /// </summary>
        /// <returns></returns>
        protected override async Task InternalCancelAsync()
            {
            await Actor.CancelAsync();
            await base.CancelAsync();
            }

        #endregion Перегруженные методы

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            Actor?.Dispose();
            Actor = null;
            ViewPort = null;
            base.DisposeManagedResources();
            }

        #endregion Реализация интерфейса IDisposable
        }
    }