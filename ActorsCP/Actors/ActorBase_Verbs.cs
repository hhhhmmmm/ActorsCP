using System;
using System.Text;
using System.Threading.Tasks;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Частичная реализация ActorBase
    /// </summary>
    public partial class ActorBase
        {
        #region Набор методов для переопределения глаголов

        /// <summary>
        /// Запуск
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected virtual Task<bool> InternalStartAsync()
            {
            return CompletedTaskBoolTrue;
            }

        /// <summary>
        /// Остановка
        /// </summary>
        /// <returns>true если все хорошо</returns>
        protected virtual Task<bool> InternalStopAsync()
            {
            return CompletedTaskBoolTrue;
            }

        /// <summary>
        /// Перегружаемая функция для выполнения некоторых действий
        /// </summary>
        /// <returns>true если объект все выполнил успешно</returns>
        protected virtual Task<bool> InternalRunAsync()
            {
            return CompletedTaskBoolTrue;
            }

        /// <summary>
        /// Метод вызывается до отправки сигнала OnActorTerminated и предназначен для очистки объекта
        /// от хранимых в нем временных данных. Также вызывается из Dispose()
        /// </summary>
        /// <param name="fromDispose">Вызов из Dispose()</param>
        protected virtual Task<bool> InternalRunCleanupBeforeTerminationAsync(bool fromDispose)
            {
            return CompletedTaskBoolTrue;
            }

        #endregion Набор методов для переопределения глаголов

        #region Свойства

        /// <summary>
        /// Запущено
        /// </summary>
        public bool IsStarted
            {
            get
                {
                switch (State)
                    {
                    case ActorState.Started:
                    case ActorState.Running:
                        {
                        return true;
                        }
                    default:
                        {
                        return false;
                        }
                    }
                }
            }

        /// <summary>
        /// Остановлено
        /// </summary>
        public bool IsStopped
            {
            get
                {
                return !IsStarted;
                }
            }

        /// <summary>
        /// Выполняется
        /// </summary>
        public bool IsRunning
            {
            get
                {
                return State == ActorState.Running;
                }
            }

        /// <summary>
        /// В ожидании
        /// </summary>
        public bool IsPending
            {
            get
                {
                return State == ActorState.Pending;
                }
            }

        /// <summary>
        /// Завершен
        /// </summary>
        public bool IsTerminated
            {
            get
                {
                return State == ActorState.Terminated;
                }
            }

        #endregion Свойства

        #region Глаголы

        /// <summary>
        /// Запуск
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StartAsync()
            {
            try
                {
                if (IsTerminated)
                    {
                    OnActorActionError("Попытка запуска завершенного объекта");
                    return false;
                    }

                if (IsCanceled)
                    {
                    OnActorActionError("Выполнение объекта отменено");
                    return false;
                    }

                if (IsStarted)
                    {
                    return true;
                    }
                OnActorAction($"Запуск {Name}...");
                var bres = await InternalStartAsync().ConfigureAwait(false);
                if (bres)
                    {
                    SetActorState(ActorState.Started);
                    OnActorAction($"{Name} успешно запущен");
                    return true;
                    }
                OnActorActionError($"Ошибка запуска {Name}");
                return false;
                }
            catch (Exception e)
                {
                OnActorThrownAnException(e);
                await TerminateAsync().ConfigureAwait(false);
                return false;
                }
            }

        /// <summary>
        /// Остановка
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopAsync()
            {
            try
                {
                if (IsTerminated && (IsStarted || IsRunning))
                    {
                    OnActorActionError("Попытка остановки завершенного объекта");
                    return false;
                    }

                if (IsTerminated)
                    {
                    return false;
                    }

                if (!IsStarted && !IsRunning)
                    {
                    return true;
                    }

                OnActorAction($"Остановка {Name}...");
                var bres = await InternalStopAsync().ConfigureAwait(false);
                if (bres)
                    {
                    OnActorAction($"{Name} остановлен {ExecutionTime.TimeIntervalWithComment}");
                    SetActorState(ActorState.Stopped);
                    return true;
                    }
                return false;
                }
            catch (Exception e)
                {
                OnActorThrownAnException(e);
                await TerminateAsync().ConfigureAwait(false);
                return false;
                }
            }

        /// <summary>
        /// Выполнить некоторую работу. Внутри себя метод Run() вызывает перегружаемый метод InternalRunAsync()
        /// </summary>
        /// <returns>true если работа успешно завершена</returns>
        public async Task<bool> RunAsync()
            {
            bool bres = false;
            try
                {
                if (IsTerminated)
                    {
                    OnActorActionError("Попытка запуска завершенного объекта");
                    return false;
                    }

                if (IsCanceled)
                    {
                    OnActorActionError("Выполнение объекта отменено");
                    return false;
                    }

                if (IsRunning)
                    {
                    OnActorActionError("Попытка запуска выполняющегося объекта");
                    return false;
                    }

                if (RunOnlyOnce && HasBeenRun)
                    {
                    OnActorActionError("Повторный запуск запрещен");
                    return false;
                    }

                #region Выполнение

                using (var gt = new ActorDisposableTime($"выполнения '{Name}'", OnActorAction))
                    {
                    if (!IsStarted)
                        {
                        bres = await StartAsync().ConfigureAwait(false);
                        if (!bres)
                            {
                            return false;
                            }
                        }

                    _executionTime.SetStartDate();
                    SetActorState(ActorState.Running);

                    HasBeenRun = true;

                    var runtask = InternalRunAsync();
                    await runtask.ConfigureAwait(false);
                    _executionTime.SetEndDate();

                    if (IsStarted || IsRunning)
                        {
                        if (RunOnlyOnce)
                            {
                            bres = await StopAsync().ConfigureAwait(false);
                            if (!bres)
                                {
                                await TerminateAsync().ConfigureAwait(false);
                                return false;
                                }
                            }
                        else // можно запускать несколько раз
                            {
                            SetActorState(ActorState.Started);
                            }
                        }

                    if (RunOnlyOnce)
                        {
                        bres = await TerminateAsync().ConfigureAwait(false);
                        if (!bres)
                            {
                            return false;
                            }
                        }

                    return runtask.Result;
                    } // end using

                #endregion Выполнение
                } // end try
            catch (OperationCanceledException) // Выполнение отменено
                {
                OnOperationCanceledException();
                return false;
                } // end catch
            catch (AggregateException ae)
                {
                _executionTime.SetEndDate();

                foreach (Exception e in ae.InnerExceptions)
                    {
                    OnActorThrownAnException(e);
                    }
                await TerminateAsync().ConfigureAwait(false);
                return false;
                } // end catch
            catch (Exception e)
                {
                _executionTime.SetEndDate();

                OnActorThrownAnException(e);
                await TerminateAsync().ConfigureAwait(false);
                return false;
                } // end catch
            }

        /// <summary>
        /// Полная и окончательная остановка
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TerminateAsync()
            {
            if (State == ActorState.Terminated)
                {
                return true;
                }
            try
                {
                var bres = await RunCleanupBeforeTerminationAsync(false).ConfigureAwait(false);
                return bres;
                }
            catch (Exception ex)
                {
                OnActorThrownAnException(ex);
                return false;
                }
            finally
                {
                SetActorState(ActorState.Terminated);
                }
            }

        #endregion Глаголы
        } // end class ActorBase
    } // end namespace ActorsCP.Actors