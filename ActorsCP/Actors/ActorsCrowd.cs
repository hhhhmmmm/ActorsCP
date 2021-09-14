using System;
using System.Collections.Generic;
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
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorsCrowd()
            {
            SetName("Набор объектов");
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

            //var array = _waiting.ToArray();
            // await TasksHelper.ForEachAsyncConcurrentAutoAsync<ActorBase>(array, (x) => { return x.RunAsync(); });
            bresult = await RunWithLimits(_waiting);
            return bresult;
            }

        #endregion Перегруженные методы

        /// <summary>
        /// Выполнить без ограничения по параллельности
        /// </summary>
        /// <param name="actorsList">Список объектов</param>
        /// <returns>true если все объекты вернули true</returns>
        private static async Task<bool> RunWithoutLimits(IEnumerable<ActorBase> actorsList)
            {
            var runningTasks = new List<Task<bool>>();
            foreach (var actor in actorsList)
                {
                var task = actor.RunAsync();
                runningTasks.Add(task);
                }

            var results = await Task.WhenAll(runningTasks);

            var bres = true;

            #region Изучаем результаты

            //foreach (var result in results)
            //    {
            //    if (!result)
            //        {
            //        bres = false;
            //        break;
            //        }
            //    }

            #endregion Изучаем результаты

            return bres;
            }

        /// <summary>
        /// https://metanit.com/sharp/tutorial/11.8.php
        /// </summary>
        /// <param name="actorsList">Список объектов</param>
        /// <param name="maxDegreeOfParallelism"></param>
        /// <returns></returns>
        public async Task<bool> RunWithLimits(IEnumerable<ActorBase> actorsList)
            {
            int maxDegreeOfParallelism = Environment.ProcessorCount;
            return await RunWithLimits(actorsList, maxDegreeOfParallelism);
            }

        /// <summary>
        /// https://metanit.com/sharp/tutorial/11.8.php
        /// </summary>
        /// <param name="actorsList">Список объектов</param>
        /// <param name="maxDegreeOfParallelism"></param>
        /// <returns></returns>
        public async Task<bool> RunWithLimits(IEnumerable<ActorBase> actorsList, int maxDegreeOfParallelism)
            {
            var runningTasks = new List<Task<bool>>();

            // первый параметр указывает, какому числу объектов изначально будет доступен семафор
            // а второй параметр указывает, какой максимальное число объектов будет использовать данный семафор
            using (var semaphoreSlim = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism))
                {
                #region Локальные функции

                // Освободить светофор по завершении задачи
                void ReleaseSemaphoreOnCompletion(Task<bool> actorRunTask)
                    {
                    semaphoreSlim.Release();
                    }

                #endregion Локальные функции

                var localActorsList = new List<ActorBase>();

                localActorsList.AddRange(actorsList);

                foreach (var actor in localActorsList)
                    {
                    // В начале для ожидания получения семафора используется метод semaphoreSlim.WaitAsync()
                    // При этом увеличивается количество запущенных задач
                    // или ожидается если их больше чем лимит
                    if (CancellationTokenSource != null)
                        {
                        await semaphoreSlim.WaitAsync(CancellationTokenSource.Token);
                        }
                    else
                        {
                        await semaphoreSlim.WaitAsync();
                        }

                    // Получено одно место светофора
                    // запускаем задачу
                    var tb = actor.RunAsync();
                    // настраиваем задачу
                    await tb.ContinueWith(ReleaseSemaphoreOnCompletion);
                    await tb.ConfigureAwait(false);

                    runningTasks.Add(tb);
                    } // end foreach

                // Ожидаем завершения всех задач
                await Task.WhenAll(runningTasks.ToArray());
                } // end using

            var bres = true;

            #region Изучаем результаты

            foreach (var result in runningTasks)
                {
                if (!result.Result)
                    {
                    bres = false;
                    break;
                    }
                }

            #endregion Изучаем результаты

            return bres;
            }

        // отпустить сфетофор
        // action is completed, so decrement the number of currently running tasks
        //Func<ActorBase, Task<bool>> RunFunc = async (ActorBase item) =>
        //    {
        //        var tb = item.RunAsync();
        //        await tb.ContinueWith(ReleaseSemaphoreOnCompletion);
        //        return tb.Result;
        //    };
        }
    }