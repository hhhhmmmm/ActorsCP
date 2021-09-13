using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActorsCP.Helpers;

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

            var array = _waiting.ToArray();
            await TasksHelper.ForEachAsyncConcurrentAutoAsync<ActorBase>(array, (x) => { return x.RunAsync(); });
            return bresult;
            }

        #endregion Перегруженные методы

        public static async Task ForEachAsyncConcurrentAsync<T>(
            IEnumerable<T> enumerable,
            Func<T, Task> action,
            int? maxDegreeOfParallelism = null)
            {
            if (action == null)
                {
                throw new ArgumentNullException(nameof(action));
                }

            if (maxDegreeOfParallelism.HasValue)
                {
                using (var semaphoreSlim = new SemaphoreSlim(
                    maxDegreeOfParallelism.Value, maxDegreeOfParallelism.Value))
                    {
                    var tasksWithThrottler = new List<Task>();

                    foreach (var item in enumerable)
                        {
                        // Increment the number of currently running tasks and wait if they are more than limit.
                        await semaphoreSlim.WaitAsync();

                        tasksWithThrottler.Add(Task.Run(async () =>
                        {
                            await action(item).ContinueWith(res =>
                            {
                                // action is completed, so decrement the number of currently running tasks
                                semaphoreSlim.Release();
                            });
                        }));
                        }

                    // Wait for all tasks to complete.
                    await Task.WhenAll(tasksWithThrottler.ToArray());
                    }
                }
            else
                {
                await Task.WhenAll(enumerable.Select(item => action(item)));
                }
            }
        }
    }