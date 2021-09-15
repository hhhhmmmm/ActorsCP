using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Класс-набор - содержит в себе несколько объектов и обрабатывает их ОДНОВРЕМЕННО
    /// </summary>
    public class ActorsCrowd : ActorsSet
        {
        //
        // Про семафоры
        // https://metanit.com/sharp/tutorial/11.8.php
        //
        // Тесты:         /// XEON: 50000 T - 36 секунд, F  - 59 секунд
        // Тесты:         /// XEON: 20000 T - 5.6 секунд, F - 9.7 секунд
        // Тесты:         /// XEON: 10000 T - 1.8 секунд, F - 3.0 секунд

        // Тесты:         /// HM: 50000 T - 1 минута,   F  - 1.3 минуты
        // Тесты:         /// HM: 20000 T - 9.2 секунд, F - 10.9 секунд
        // Тесты:         /// HM: 10000 T - 3.2 секунд, F -  3.5 секунд

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorsCrowd()
            {
            SetName("Толпа объектов");
            SetMaxDegreeOfParallelism(Environment.ProcessorCount);
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название очереди объектов</param>
        public ActorsCrowd(string name) : base(name)
            {
            }

        #endregion Конструкторы

        #region Перегруженные методы

        /// <summary>
        /// Перегружаемая функция для выполнения некоторых действий
        /// </summary>
        /// <returns>true если объект все выполнил успешно</returns>
        protected override async Task<bool> InternalRunAsync()
            {
            bool bresult = true;
            if (WaitingCount == 0)
                {
                return true;
                }

            bresult = await RunInParallel(WaitingList);
            return bresult;
            }

        #endregion Перегруженные методы

        /// <summary>
        /// Установить параллелизм (количество одновременно выполняющихся объектов)
        /// </summary>
        /// <param name="maxDegreeOfParallelism"></param>
        public void SetMaxDegreeOfParallelism(int? maxDegreeOfParallelism)
            {
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
            }

        #region Свойства

        /// <summary>
        /// Параллелизм (количество одновременно выполняющихся объектов)
        /// </summary>
        public int? MaxDegreeOfParallelism
            {
            get;
            private set;
            }

        #endregion Свойства

        #region Приватные методы

        /// <summary>
        /// Выполнить парллельно
        /// </summary>
        /// <param name="actorsList">Список объектов</param>
        /// <returns></returns>
        private async Task<bool> RunInParallel(HashSet<ActorBase> actorsList)
            {
            int nCount = actorsList.Count;
            ActorTime actorTime = default;
            try
                {
                actorTime.SetStartDate();
                if (!MaxDegreeOfParallelism.HasValue)
                    {
                    return await RunInParallelWithoutLimits(actorsList);
                    }
                else
                    {
                    return await RunInParallelWithLimits(actorsList);
                    }
                }
            finally
                {
                actorTime.SetEndDate();
#if DEBUG
                var result = actorTime.GetTimeIntervalWithComment(nCount);
                OnActorAction(result);
#endif // DEBUG
                }
            }

        /// <summary>
        /// Выполнить без ограничения по параллельности
        /// Работает примерно на 15% медленнее чем RunInParallelWithLimits()
        /// 10.7-9.0 10.5-9.4 11.1-9.4
        /// </summary>
        /// <param name="actorsList">Список объектов</param>
        /// <returns>true если все объекты вернули true</returns>
        private static async Task<bool> RunInParallelWithoutLimits(HashSet<ActorBase> actorsList)
            {
            int nCount = actorsList.Count;

            // var runningTasks = new List<Task<bool>>(nCount);
            var runningTasks = new ConcurrentQueue<Task<bool>>();

            var localActorsList = new List<ActorBase>(nCount);

            localActorsList.AddRange(actorsList);

            Parallel.ForEach(localActorsList, (ActorBase actor) =>
            {
                var task = actor.RunAsync();
                runningTasks.Enqueue(task);
            });

            var results = await Task.WhenAll(runningTasks).ConfigureAwait(false);

            #region Изучаем результаты

            if (results.Any((bool x) => { return x == false; })) // если есть хоть один равный false
                {
                return false;
                }

            return true;

            #endregion Изучаем результаты
            }

        #endregion Приватные методы

        /// <summary>
        /// Выполнить c ограничением по параллельности
        /// </summary>
        /// <param name="actorsList">Список объектов</param>
        /// <returns></returns>
        public async Task<bool> RunInParallelWithLimits(HashSet<ActorBase> actorsList)
            {
            int nCount = actorsList.Count;

            var runningTasks = new ConcurrentQueue<Task<bool>>();

            // первый параметр указывает, какому числу объектов изначально будет доступен семафор
            // а второй параметр указывает, какой максимальное число объектов будет использовать данный семафор
            using (var semaphoreSlim = new SemaphoreSlim(MaxDegreeOfParallelism.Value, MaxDegreeOfParallelism.Value))
                {
                #region Локальные функции

                // Освободить светофор по завершении задачи
                void ReleaseSemaphoreOnCompletion(Task<bool> actorRunTask)
                    {
                    semaphoreSlim.Release();
                    }

                #endregion Локальные функции

                var localActorsQueue = new ConcurrentQueue<ActorBase>(actorsList);

                foreach (var actor in localActorsQueue)
                    {
                    // В начале для ожидания получения семафора используется метод semaphoreSlim.WaitAsync()
                    // При этом увеличивается количество запущенных задач
                    // или ожидается если их больше чем лимит
                    if (CancellationTokenSource != null)
                        {
                        await semaphoreSlim.WaitAsync(CancellationTokenSource.Token).ConfigureAwait(false);
                        }
                    else
                        {
                        await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                        }

                    // Получено одно место светофора, запускаем задачу
                    var task = actor.RunAsync();
                    // настраиваем задачу
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    task.ContinueWith(ReleaseSemaphoreOnCompletion);
                    task.ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                    runningTasks.Enqueue(task);
                    } // end foreach

                // Ожидаем завершения всех задач
                await Task.WhenAll(runningTasks.ToArray()).ConfigureAwait(false);
                } // end using

            #region Изучаем результаты

            if (runningTasks.Any((Task<bool> x) => { return x.Result == false; })) // если есть хоть один равный false
                {
                return false;
                }

            return true;

            #endregion Изучаем результаты
            }
        }
    }