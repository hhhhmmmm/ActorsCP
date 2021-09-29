using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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
        /// Очередь находится в состоянии завершения
        /// </summary>
        private ManualResetEventSlim _terminatingEvent = new ManualResetEventSlim();

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
        /// Таймаут ожидания очереди, ms
        /// </summary>
        private int _queueTimeout = 100;

        /// <summary>
        /// Внешний обработчик сообщений
        /// </summary>
        private Action<T> _externalMessageHandler;

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
        public QueueBufferT(Action<T> externalMessageHandler)
            {
            if (externalMessageHandler == null)
                {
                throw new ArgumentNullException($"{nameof(externalMessageHandler)} не может быть null");
                }

            _externalMessageHandler = externalMessageHandler;
            _queue = new ConcurrentQueue<T>();

            _queueSemaphoreSlim = new SemaphoreSlim(0, int.MaxValue);
            //_queueSemaphoreSlim = new SemaphoreSlim(1, int.MaxValue);
            // _queueSemaphoreSlim = new SemaphoreSlim(int.MaxValue, int.MaxValue); // Adding the specified count to the semaphore would cause it to exceed its maximum count
            _queueTask = Task.Run(ProcessMessageFromQueueLoop);
            }

        #endregion Конструкторы

        private readonly object Locker = new object();

        /// <summary>
        /// Цикл обработки сообщений
        /// </summary>
        private void ProcessMessageFromQueueLoop()
            {
            int nDebugCounter = 0;

            var waitHandles = new WaitHandle[2];
            waitHandles[0] = _terminatingEvent.WaitHandle;
            waitHandles[1] = _queueSemaphoreSlim.AvailableWaitHandle;

            while (true)
                {
                int waitResult = WaitHandle.WaitAny(waitHandles);

                var str = $"_queue.Count ={_queue.Count}, _queueSemaphoreSlim.CurrentCount = {_queueSemaphoreSlim.CurrentCount}";
                Debug.WriteLine(str);

                switch (waitResult)
                    {
                    case 0: // _terminatingEvent
                        {
                        if (_queue.IsEmpty)
                            {
                            return;
                            }
                        break;
                        }
                    case 1: // _queueSemaphoreSlim
                        {
                        lock (Locker)
                            {
                            if (_queue.IsEmpty)
                                {
                                nDebugCounter++;
                                //Debug.WriteLine($"nDebugCounter = {nDebugCounter}, _queue.IsEmpty но семафор сработал - такого быть не должно");
                                throw new Exception("_queue.IsEmpty но семафор сработал - такого быть не должно");
                                }

                            //_queueSemaphoreSlim.Release();
                            T item;
                            bool bres = _queue.TryDequeue(out item);
                            if (!bres)
                                {
                                // nDebugCounter++;
                                // Debug.WriteLine("nDebugCounter = {nDebugCounter}, Ошибка извлечения из очереди");
                                _queueSemaphoreSlim.Release();
                                continue;
                                // throw new Exception("Ошибка извлечения из очереди");
                                }

                            //if ((!(_queue.IsEmpty)) && _queue.TryDequeue(out T item))
                                {
                                _externalMessageHandler.Invoke(item);
                                Interlocked.Increment(ref _queueBufferStatistics.ProcessedMessages);
                                }
                            } // end lock  Locker
                        break;
                        }
                    }
                }
            }

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

            lock (Locker)
                {
                _queue.Enqueue(item);

                Interlocked.Increment(ref _queueBufferStatistics.AddedMessages);

                _queueSemaphoreSlim.Release();
                }
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
        /// Ожидание опустошения очереди
        /// </summary>
        /// <returns></returns>
        public async Task WaitAsync()
            {
            if (IsTerminated)
                {
                return;
                }

            while (true)
                {
                if (_queue.IsEmpty)
                    {
                    return;
                    }
                await Task.Delay(_queueTimeout);
                }
            }

        /// <summary>
        /// Ожидание опустошения очереди
        /// </summary>
        public void Wait()
            {
            if (IsTerminated)
                {
                return;
                }
            WaitAsync().Wait();
            }

        /// <summary>
        /// Завершение очереди
        /// </summary>
        public void Terminate()
            {
            TerminateAsync().Wait();
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

            if (_queueTask.IsFaulted)
                {
                var exception = _queueTask.Exception;
                if (exception != null)
                    {
                    throw exception;
                    }
                }

            if (!_queueTask.IsFaulted)
                {
                _queueTask.Wait();
                }
            _queueTask.Dispose();
            _queueTask = null;
            _queueSemaphoreSlim?.Dispose();
            _queueSemaphoreSlim = null;

            _terminatingEvent?.Dispose();
            _terminatingEvent = null;

            if (!_queue.IsEmpty)
                {
                throw new Exception($"В очереди осталось {_queue.Count} сообщений");
                }

            _queue = null;
            _externalMessageHandler = null;

            _isTerminated = true;
            }

        #endregion Ожидание и остановка
        } // end class QueueBufferT
    } // end namespace ActorsCP.Helpers