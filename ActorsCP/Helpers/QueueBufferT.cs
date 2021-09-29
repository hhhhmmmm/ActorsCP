using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Внутренняя статистика буфера
    /// </summary>
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
        /// Очередь сообщения
        /// </summary>
        private ConcurrentQueue<T> _queue;

        /// <summary>
        /// Семафор для очереди
        /// </summary>
        private SemaphoreSlim _queueSemaphoreSlim;

        /// <summary>
        /// Вьюпорт находится в состоянии завершения
        /// </summary>
        private bool _isTerminating;

        /// <summary>
        /// Задача-обработчик очереди сообщений
        /// </summary>
        private Task _queueTask;

        /// <summary>
        /// Внутренняя статистика буфера
        /// </summary>
        private QueueBufferStatistics _queueBufferStatistics;

        /// <summary>
        /// Таймаут ожидания очереди, ms
        /// </summary>
        private int _queueTimeout = 100;

        /// <summary>
        /// Внешний обработчик сообщений
        /// </summary>
        private Action<T> _processExtractedMessage;

        #endregion Приватные мемберы

        #region Свойства

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

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public QueueBufferT()
            {
            _queue = new ConcurrentQueue<T>();
            _queueSemaphoreSlim = new SemaphoreSlim(1, int.MaxValue);
            _queueTask = Task.Run(ProcessMessageFromQueueLoop);
            }

        #endregion Конструкторы

        #region Добавление сообщений

        /// <summary>
        /// Добавить данные в очередь на обработку
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Add(T item)
            {
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
            InternalTerminate();
            base.DisposeManagedResources();
            }

        #endregion Реализация вызываемых методов интерфейса IDisposable

        /// <summary>
        ///
        /// </summary>
        private void InternalTerminate()
            {
            if (!_queueTask.IsFaulted)
                {
                _queueTask.Wait();
                }
            _queueTask.Dispose();
            _queueTask = null;
            _queueSemaphoreSlim?.Dispose();
            _queueSemaphoreSlim = null;

            if (!_queue.IsEmpty)
                {
                throw new Exception($"В очереди осталось {_queue.Count} сообщений");
                }

            _queue = null;

            _processExtractedMessage = null;
            }

        /// <summary>
        /// Цикл обработки сообщений
        /// </summary>
        private void ProcessMessageFromQueueLoop()
            {
            while (true)
                {
                _queueSemaphoreSlim.Wait(QueueTimeout);

                if ((!(_queue.IsEmpty)) && _queue.TryDequeue(out T item))
                    {
                    _processExtractedMessage?.Invoke(item);

                    Interlocked.Increment(ref _queueBufferStatistics.ProcessedMessages);
                    }
                else
                    {
                    if (_isTerminating)
                        {
                        if (_queue.IsEmpty)
                            {
                            return;
                            }
                        }
                    }
                }
            }
        } // end class QueueBufferT
    } // end namespace ActorsCP.Helpers