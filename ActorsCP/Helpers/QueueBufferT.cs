using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ActorsCP.Logger;
using ActorsCP.Options;

namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Внутренняя статистика буфера
    /// </summary>
    [DebuggerDisplay("Messages: Added  = {AddedMessages}, Processed = {ProcessedMessages}")]
    public struct QueueBufferStatistics
        {
        /// <summary>
        /// Количество добавленных сообщений
        /// </summary>
        public volatile int AddedMessages;

        /// <summary>
        /// Количество обработанных сообщений
        /// </summary>
        public volatile int ProcessedMessages;
        }

    /// <summary>
    /// Буфер основанный на очереди
    /// </summary>
    public class QueueBufferT<T> : DisposableImplementation<QueueBufferT<T>> where T : class
        {
        #region Приватные мемберы

        /// <summary>
        /// В опциях включена отладка
        /// </summary>
        private readonly bool _isDebugging;

        /// <summary>
        /// Очередь сообщения
        /// </summary>
        private readonly ConcurrentQueue<T> _queue;

        /// <summary>
        /// Семафор для очереди
        /// </summary>
        private readonly SemaphoreSlim _queueSemaphoreSlim;

        /// <summary>
        /// Очередь находится в состоянии завершения
        /// </summary>
        private readonly ManualResetEventSlim _terminatingEvent = new ManualResetEventSlim();

        /// <summary>
        /// Очередь завершена
        /// </summary>
        private bool _isTerminated;

        /// <summary>
        /// Задача-обработчик очереди сообщений
        /// </summary>
        private Task _queueTask;

        /// <summary>
        /// Внутренняя статистика буфера
        /// </summary>
        private QueueBufferStatistics _queueBufferStatistics;

        /// <summary>
        /// Источник отмены задачи
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Таймаут ожидания очереди, ms
        /// </summary>
        private int _queueTimeout = 10;

        /// <summary>
        /// Внешний обработчик сообщений
        /// </summary>
        private readonly Action<T> _externalMessageHandler;

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly IActorLogger _logger;

        #endregion Приватные мемберы

        #region Свойства

        /// <summary>
        /// В опциях включена отладка
        /// </summary>
        public bool IsDebugging
            {
            get
                {
                return _isDebugging;
                }
            }

        /// <summary>
        ///
        /// </summary>
        private IActorLogger Logger
            {
            get
                {
                return _logger;
                }
            }

        /// <summary>
        /// Таймаут ожидания очереди, ms
        /// </summary>
        public int QueueTimeout
            {
            get
                {
                return _queueTimeout;
                }
            set
                {
                if (value < 0)
                    {
                    throw new ArgumentException("Значение должно быть больше 0");
                    }
                _queueTimeout = value;
                }
            }

        /// <summary>
        /// Внутренняя статистика буфера
        /// </summary>
        public QueueBufferStatistics Statistics
            {
            get
                {
                return _queueBufferStatistics;
                }
            }

        /// <summary>
        /// Очередь находится в состоянии завершения
        /// </summary>
        public bool IsTerminating
            {
            get
                {
                if (_terminatingEvent == null)
                    {
                    return false;
                    }
                return _terminatingEvent.IsSet;
                }
            }

        /// <summary>
        /// Очередь завершена
        /// </summary>
        public bool IsTerminated
            {
            get
                {
                return _isTerminated;
                }
            }

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="externalMessageHandler">Внешний обработчик сообщений</param>
        private QueueBufferT(Action<T> externalMessageHandler)
            {
            if (externalMessageHandler == null)
                {
                throw new ArgumentNullException($"{nameof(externalMessageHandler)} не может быть null");
                }

            #region Отладка

            var debugOptions = GlobalActorDebugOptions.GetInstance();
            debugOptions.GetBool(ActorDebugKeywords.QueueBufferT_Debug, out _isDebugging);

            #endregion Отладка

            _externalMessageHandler = externalMessageHandler;
            _queue = new ConcurrentQueue<T>();
            _queueSemaphoreSlim = new SemaphoreSlim(0, int.MaxValue);

            // Логгер
            _logger = GlobalActorLogger.GetInstance();
            }

        #endregion Конструкторы

        #region Отладка

        /// <summary>
        /// Вывести отладочный текст
        /// </summary>
        /// <param name="text">Отладочный текст</param>
        public void LogDebug(string text)
            {
            if (IsDebugging)
                {
                Logger?.LogDebug("QBT - " + text);
                }
            }

        #endregion Отладка

        /// <summary>
        /// Цикл обработки сообщений
        /// </summary>
        private async Task InternalProcessMessageFromQueueLoop()
            {
            while (true)
                {
                if (IsTerminated)
                    {
                    return;
                    }

                if (_queueSemaphoreSlim == null)
                    {
                    return;
                    }

                var hasData = await _queueSemaphoreSlim.WaitAsync(_queueTimeout, _cancellationTokenSource.Token).ConfigureAwait(false);
                if (!hasData)
                    {
                    if (_terminatingEvent.IsSet)
                        {
                        if (_queue.IsEmpty)
                            {
                            return;
                            }
                        }
                    continue;
                    }
                // данные есть

                bool bres = _queue.TryDequeue(out T item);
                if (!bres) // извлечь не удалось
                    {
                    _queueSemaphoreSlim.Release();
                    // throw new Exception("Ошибка извлечения из очереди");
                    continue;
                    }

                _externalMessageHandler.Invoke(item);
                Interlocked.Increment(ref _queueBufferStatistics.ProcessedMessages);
                }
            }

        /// <summary>
        /// Очистка по завершению
        /// </summary>
        private void InternalClearMessageLoop()
            {
            if (IsDebugging)
                {
                LogDebug("Начало InternalClearMessageLoop()");
                }

            if (!_queue.IsEmpty)
                {
                throw new Exception($"В очереди осталось {_queue.Count} сообщений - нехорошо");
                }

            //_queueSemaphoreSlim.Dispose();
            //_queueSemaphoreSlim = null;

            //_terminatingEvent.Dispose();
            //_terminatingEvent = null;

            if (IsDebugging)
                {
                LogDebug("Конец InternalClearMessageLoop()");
                }
            }

        /// <summary>
        /// Запустить цикл обработки сообщений
        /// </summary>
        public void RunProcessingLoop()
            {
            if (IsDebugging)
                {
                LogDebug("Начало RunProcessingLoop()");
                }

            _queueTask = Task.Run(async () =>
            {
                if (IsDebugging)
                    {
                    LogDebug("До InternalProcessMessageFromQueueLoop()");
                    }

                await InternalProcessMessageFromQueueLoop();

                if (IsDebugging)
                    {
                    LogDebug("После InternalProcessMessageFromQueueLoop()");
                    }

                _cancellationTokenSource.Cancel();

                if (IsDebugging)
                    {
                    LogDebug("До InternalClearMessageLoop()");
                    }

                InternalClearMessageLoop();

                if (IsDebugging)
                    {
                    LogDebug("После InternalClearMessageLoop()");
                    }

                if (IsDebugging)
                    {
                    LogDebug("Завершение RunProcessingLoop() -> Task.Run");
                    }
            });

            if (IsDebugging)
                {
                LogDebug("Конец RunProcessingLoop()");
                }
            }

        /// <summary>
        /// Создать экземпляр очереди
        /// </summary>
        /// <param name="externalMessageHandler">Внешний обработчик сообщений</param>
        /// <returns></returns>
        public static QueueBufferT<T> Create(Action<T> externalMessageHandler)
            {
            var ret = new QueueBufferT<T>(externalMessageHandler);
            ret.RunProcessingLoop();

            return ret;
            }

        #region Добавление сообщений

        /// <summary>
        /// Добавить данные в очередь на обработку
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Add(T item)
            {
            if (IsTerminated || IsTerminating)
                {
                throw new InvalidOperationException("Очередь закрыта для добавления");
                }

            if (item == null)
                {
                throw new ArgumentNullException($"{nameof(item)} не может быть null");
                }

            if (_queue == null)
                {
                throw new InvalidOperationException("_queue == null");
                }

            _queue.Enqueue(item);
            Interlocked.Increment(ref _queueBufferStatistics.AddedMessages);
            _queueSemaphoreSlim.Release();
            }

        #endregion Добавление сообщений

        #region Реализация вызываемых методов интерфейса IDisposable

        /// <summary>
        /// Освободить управляемые ресурсы
        /// </summary>
        protected override void DisposeManagedResources()
            {
            Terminate();
            base.DisposeManagedResources();
            }

        #endregion Реализация вызываемых методов интерфейса IDisposable

        #region Ожидание и остановка

        /// <summary>
        /// Выкинуть исключения если они есть
        /// </summary>
        private void ThrowExceptionsIfAny()
            {
            if (_queueTask.IsFaulted)
                {
                var exception = _queueTask.Exception;
                if (exception != null)
                    {
                    throw exception;
                    }
                }
            }

        /// <summary>
        /// Ожидание опустошения очереди
        /// </summary>
        /// <returns></returns>
        public async Task WaitAsync()
            {
            if (IsDebugging)
                {
                LogDebug("Начало WaitAsync()");
                }
            try
                {
                if (IsTerminated || IsTerminating)
                    {
                    return;
                    }

                while (true)
                    {
                    ThrowExceptionsIfAny();

                    if (_queue.IsEmpty && _queueSemaphoreSlim.CurrentCount == 0)
                        {
                        return;
                        }
                    await Task.Delay(_queueTimeout);
                    }
                } // end try
            finally
                {
                if (IsDebugging)
                    {
                    LogDebug("Конец WaitAsync()");
                    }
                } // end finally
            }

        /// <summary>
        /// Ожидание опустошения очереди
        /// </summary>
        public void Wait()
            {
            if (IsDebugging)
                {
                LogDebug("Начало Wait()");
                }
            try
                {
                WaitAsync().Wait();
                }
            finally
                {
                if (IsDebugging)
                    {
                    LogDebug("Конец Wait()");
                    }
                }
            }

        /// <summary>
        /// Завершение очереди
        /// </summary>
        public void Terminate()
            {
            if (IsDebugging)
                {
                LogDebug("Начало Terminate()");
                }
            TerminateAsync().Wait();
            if (IsDebugging)
                {
                LogDebug("Конец Terminate()");
                }
            }

        /// <summary>
        /// Завершение очереди
        /// </summary>
        /// <returns></returns>
        public async Task TerminateAsync()
            {
            if (IsTerminated)
                {
                return;
                }

            _terminatingEvent.Set();
            await WaitAsync(); // ожидание опустошения очереди

            // здесь по идее нужно дропнуть внутренние объекты
            _cancellationTokenSource?.Dispose();

            _isTerminated = true;
            }

        #endregion Ожидание и остановка
        } // end class QueueBufferT
    } // end namespace ActorsCP.Helpers