using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Частичная реализация ActorBase
    /// </summary>
    public partial class ActorBase
        {
        /// <summary>
        /// Источник отмены задачи
        /// </summary>
        private readonly CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

        #region Свойства

        /// <summary>
        /// Источник отмены выполнения
        /// </summary>
        public CancellationTokenSource CancellationTokenSource
            {
            get
                {
                return m_CancellationTokenSource;
                }
            }

        /// <summary>
        /// Выполнение отменено
        /// </summary>
        public bool IsCanceled
            {
            get;
            private set;
            }

        #endregion Свойства

        private const string _CancelText = "Выполнение объекта отменено";

        /// <summary>
        /// Отменяет выполнение объекта
        /// и безусловно завершает его работу
        /// </summary>
        public virtual async Task CancelAsync()
            {
            if (IsCanceled)
                {
                return;
                }

            if (IsTerminated) // если хочется поставить на завершенном объекте, то хуже уже никому не будет
                {
                if (!IsCanceled)
                    {
                    IsCanceled = true;
                    }
                return;
                }

            m_CancellationTokenSource?.Cancel();
            IsCanceled = true;

            OnActorActionWarning(_CancelText);

            if (IsStarted || IsRunning)
                {
                await StopAsync();
                }

            if (!IsTerminated)
                {
                await TerminateAsync();
                }
            }

        /// <summary>
        /// Проверить токен отмены выполнения и выбросить исключение
        /// если токен установлен
        /// </summary>
        protected void ThrowIfCancellationRequested()
            {
            m_CancellationTokenSource?.Token.ThrowIfCancellationRequested();
            }

        /// <summary>
        /// Стандартный набор действий при получении исключения OperationCanceledException
        /// </summary>
        protected async void OnOperationCanceledException()
            {
            bool bres;

            if (this.ExecutionTime.HasStartDate)
                {
                ExecutionTime.SetEndDate();
                }

            OnActorActionWarning(_CancelText);

            if (IsStarted || IsRunning)
                {
                bres = await StopAsync();
                }

            if (!IsTerminated)
                {
                bres = await TerminateAsync();
                }
            }
        } // end class ActorBase
    } // end namespace ActorsCP.Actors