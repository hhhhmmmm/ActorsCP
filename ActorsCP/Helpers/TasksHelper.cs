using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ActorsCP.Helpers
    {
    public static class TasksHelper
        {
        /// <summary>
        /// Завершенная задача типа bool
        /// </summary>
        public static readonly Task<bool> CompletedTaskBoolTrue = Task.FromResult<bool>(true);

        /// <summary>
        /// Неуспешно завершенная задача
        /// </summary>
        public static readonly Task<bool> CompletedTaskBoolFalse = Task.FromResult<bool>(false);

        /// <summary>
        /// Опции задачи по умолчанию
        /// </summary>
        public readonly static TaskCreationOptions DefaultTaskCreationOptions = TaskCreationOptions.PreferFairness;

        /// <summary>
        /// Создать TaskCompletionSource
        /// </summary>
        /// <param name="t">todo: describe t parameter on CreateTaskCompletionSource</param>
        /// <typeparam name="T">имя типа</typeparam>
        /// <returns></returns>
        public static TaskCompletionSource<T> CreateTaskCompletionSource<T>(T t)
            {
            var tcs = new TaskCompletionSource<T>(t);
            tcs.Task.ConfigureAwait(false);
            return tcs;
            }

        /// <summary>
        /// Создать задачу
        /// </summary>
        /// <param name="action">Действие которое нужно выполнить</param>
        /// <returns></returns>
        public static Task CreateTaskAsync(Action action)
            {
            if (action == null)
                {
                throw new ArgumentNullException(nameof(action), $"{nameof(action)} не может быть null");
                }

            var task = new Task(action, DefaultTaskCreationOptions);
            task.ConfigureAwait(false);
            task.Start(); // вызывает action
            return task;
            }

        /// <summary>
        /// Concurrently Executes async actions for each item of IEnumerable(T)/>
        /// </summary>
        /// <typeparam name="T">Type of IEnumerable</typeparam>
        /// <param name="enumerable">instance of IEnumerable(T)/></param>
        /// <param name="action">an async <see cref="Action" /> to execute</param>
        /// <param name="maxDegreeOfParallelism">Optional, An integer that represents the maximum degree of parallelism,
        /// Must be grater than 0</param>
        /// <returns>A Task representing an async operation</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the maxActionsToRunInParallel is less than 1</exception>
        /// <example>
        /// Sample Usage:
        /// await enumerable.ForEachAsyncConcurrent(
        /// async item =>
        ///     {
        ///     await SomeAsyncMethod(item);
        ///     },
        ///  5);
        /// </example>
        public static async Task ForEachAsyncConcurrentAsync<T>(
            this IEnumerable<T> enumerable,
            Func<T, Task> action,
            int? maxDegreeOfParallelism = null)
            {
            if (action == null)
                {
                throw new ArgumentNullException(nameof(action), $"{nameof(action)} не может быть null");
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
                        await semaphoreSlim.WaitAsync().ConfigureAwait(false);

                        tasksWithThrottler.Add(Task.Run(async () =>
                        {
                            await action(item).ContinueWith(res =>
                            {
                                // action is completed, so decrement the number of currently running tasks
                                semaphoreSlim.Release();
                            }).ConfigureAwait(false);
                        }));
                        } // end foreach

                    // Wait for all tasks to complete.
                    await Task.WhenAll(tasksWithThrottler.ToArray()).ConfigureAwait(false);
                    }
                }
            else
                {
                await Task.WhenAll(enumerable.Select(item => action(item))).ConfigureAwait(false);
                }
            }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task ForEachAsyncConcurrentAutoAsync<T>(
                   this IEnumerable<T> enumerable,
                   Func<T, Task> action
                   )
            {
            var maxDegreeOfParallelism = GetMaxDegreeOfParallelism();
            return ForEachAsyncConcurrentAsync<T>(enumerable, action, maxDegreeOfParallelism);
            }

        /// <summary>
        /// Получить макс. кол-во параллельных потоков
        /// </summary>
        /// <returns></returns>
        public static int GetMaxDegreeOfParallelism()
            {
            var maxDegreeOfParallelism = Environment.ProcessorCount;
            //maxDegreeOfParallelism = 200;
            return maxDegreeOfParallelism;
            }
        }
    }