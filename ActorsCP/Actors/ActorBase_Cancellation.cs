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
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        #region Свойства

        /// <summary>
        /// Источник отмены выполнения
        /// </summary>
        public CancellationTokenSource CancellationTokenSource
            {
            get
                {
                return _cancellationTokenSource;
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

            _cancellationTokenSource?.Cancel();
            IsCanceled = true;

            OnActorActionWarning(_CancelText);

            if (IsStarted || IsRunning)
                {
                await StopAsync().ConfigureAwait(false);
                }

            if (!IsTerminated)
                {
                await TerminateAsync().ConfigureAwait(false);
                }
            }

        /// <summary>
        /// Проверить токен отмены выполнения и выбросить исключение
        /// если токен установлен
        /// </summary>
        protected void ThrowIfCancellationRequested()
            {
            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();
            }

        /// <summary>
        /// Стандартный набор действий при получении исключения OperationCanceledException
        /// </summary>
        protected async void OnOperationCanceledException()
            {
            if (this.ExecutionTime.HasStartDate)
                {
                ExecutionTime.SetEndDate();
                }

            OnActorActionWarning(_CancelText);

            if (IsStarted || IsRunning)
                {
                await StopAsync().ConfigureAwait(false);
                }

            if (!IsTerminated)
                {
                await TerminateAsync().ConfigureAwait(false);
                }
            }
        } // end class ActorBase
    } // end namespace ActorsCP.Actors