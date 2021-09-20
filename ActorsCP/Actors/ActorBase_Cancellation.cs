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
        #region Приватные мемберы

        /// <summary>
        /// Текст сообщения
        /// </summary>
        private const string _сanceledMessageText = "Выполнение объекта отменено";

        /// <summary>
        /// Источник отмены задачи
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Приватная переменная - используется если объект уже Dispose()
        /// </summary>
        private bool _IsCanceled;

        #endregion Приватные мемберы

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
        /// Был сделан запрос на отмену выполнения
        /// </summary>
        public bool IsCancellationRequested
            {
            get
                {
                if (_cancellationTokenSource != null)
                    {
                    return _cancellationTokenSource.IsCancellationRequested;
                    }
                return _IsCanceled;
                }
            }

        #endregion Свойства

        /// <summary>
        /// Отменяет выполнение объекта
        /// и безусловно завершает его работу
        /// </summary>
        public virtual async Task CancelAsync()
            {
            if (IsCancellationRequested)
                {
                return;
                }

            if (IsTerminated) // если хочется поставить на завершенном объекте, то хуже уже никому не будет
                {
                if (!IsCancellationRequested)
                    {
                    _IsCanceled = true;
                    _cancellationTokenSource?.Cancel();
                    }
                return;
                }

            _cancellationTokenSource?.Cancel();

            _IsCanceled = true;

            OnActorActionWarning(_сanceledMessageText);

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
        /// Установить новый токен отмены
        /// </summary>
        /// <param name="Token">Новый токен отмены</param>
        public void SetCancellationToken(CancellationTokenSource tokenSource)
            {
            if (tokenSource == null)
                {
                throw new ArgumentNullException(nameof(tokenSource));
                }

            SetCancellationToken(tokenSource.Token);
            }

        /// <summary>
        /// Установить новый токен отмены
        /// </summary>
        /// <param name="token">Новый токен отмены</param>
        public virtual void SetCancellationToken(CancellationToken token)
            {
            if (_cancellationTokenSource != null && _cancellationTokenSource.Token == token)
                {
                return;
                }

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
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
            if (ExecutionTime.HasStartDate && (!ExecutionTime.HasEndDate))
                {
                ExecutionTime.SetEndDate();
                }

            OnActorActionWarning(_сanceledMessageText);

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