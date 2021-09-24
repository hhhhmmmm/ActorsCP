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
                switch (State)
                    {
                    case ActorState.Pending:
                    case ActorState.Started:
                    case ActorState.Running:
                        {
                        return false;
                        }
                    default:
                        {
                        return true;
                        }
                    }
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

                if (IsCancellationRequested)
                    {
                    OnActorActionError("Выполнение объекта отменено");
                    return false;
                    }

                if (IsStarted)
                    {
                    return true;
                    }

                if ((Verbosity & ActorVerbosity.Starting) != 0)
                    {
                    OnActorAction($"Запуск {Name}...");
                    }

                var bres = await InternalStartAsync().ConfigureAwait(false);
                if (bres)
                    {
                    SetActorState(ActorState.Started);
                    AfterStateChanged();
                    if ((Verbosity & ActorVerbosity.Started) != 0)
                        {
                        OnActorAction($"{Name} успешно запущен");
                        }
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

                if ((Verbosity & ActorVerbosity.Stopping) != 0)
                    {
                    OnActorAction($"Остановка {Name}...");
                    }
                var bres = await InternalStopAsync().ConfigureAwait(false);
                if (bres)
                    {
                    if ((Verbosity & ActorVerbosity.Stopped) != 0)
                        {
                        OnActorAction($"{Name} остановлен {ExecutionTime.TimeIntervalWithComment}");
                        }
                    SetActorState(ActorState.Stopped);
                    AfterStateChanged();
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
        /// Метод уведомительного характера для производных классов
        /// чтобы выбросить сообщение об изменении состояния
        /// </summary>
        protected virtual void AfterStateChanged()
            {
            }

        /// <summary>
        /// Выполнить некоторую работу. Внутри себя метод Run() вызывает перегружаемый метод InternalRunAsync()
        /// </summary>
        /// <returns>true если работа успешно завершена</returns>
        public async Task<bool> RunAsync()
            {
            try
                {
                if (IsTerminated)
                    {
                    OnActorActionError("Попытка запуска завершенного объекта");
                    return false;
                    }

                if (IsCancellationRequested)
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

                if (!IsStarted)
                    {
                    var bres = await StartAsync().ConfigureAwait(false);
                    if (!bres)
                        {
                        return false;
                        }
                    }

                Task<bool> runtask;
                var suppressOutput = (Verbosity & ActorVerbosity.Running) == 0 ? true : false;
                using (var gt = new ActorDisposableTime($" - выполнения '{Name}'", OnActorAction, suppressOutput))
                    {
                    _executionTime.SetStartDate();
                    SetActorState(ActorState.Running);

                    HasBeenRun = true;

                    runtask = InternalRunAsync();
                    await runtask.ConfigureAwait(false);
                    _executionTime.SetEndDate();
                    } // end using

                if (IsStarted || IsRunning)
                    {
                    if (RunOnlyOnce)
                        {
                        var bres = await StopAsync().ConfigureAwait(false);
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
                    var bres = await TerminateAsync().ConfigureAwait(false);
                    if (!bres)
                        {
                        return false;
                        }
                    }

                return runtask.Result;

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
        public virtual async Task<bool> TerminateAsync()
            {
            if (State == ActorState.Terminated)
                {
                return true;
                }
            try
                {
                // RaiseActorEvent(ActorStates.Terminated);
                // RaiseActorStateChanged(ActorStates.Terminated);
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