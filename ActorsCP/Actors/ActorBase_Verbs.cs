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
        #region Вспомогательные методы

        /// <summary>
        /// Частичная реализация - инициализация логгера
        /// </summary>
        protected virtual void InternalInitLogger()
            {
            }

        #endregion Вспомогательные методы

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
        protected virtual Task<bool> InternalRunCleanupBeforeTerminationAsync()
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
                    RaiseError("Попытка запуска завершенного объекта");
                    return false;
                    }

                if (IsStarted)
                    {
                    return true;
                    }
                RaiseMessage($"Запуск {Name}...");
                var bres = await InternalStartAsync();
                if (bres)
                    {
                    SetActorState(ActorState.Started);
                    RaiseMessage($"{Name} успешно запущен");
                    return true;
                    }
                RaiseMessage($"Ошибка запуска {Name}");
                return false;
                }
            catch (Exception e)
                {
                RaiseException(e);
                await TerminateAsync();
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
                if (IsTerminated)
                    {
                    RaiseError("Попытка запуска завершенного объекта");
                    return false;
                    }

                if (!IsStarted && !IsRunning)
                    {
                    return true;
                    }
                RaiseMessage($"Остановка {Name}...");
                var bres = await InternalStopAsync();
                if (bres)
                    {
                    RaiseMessage($"{Name} остановлен");
                    SetActorState(ActorState.Stopped);
                    return true;
                    }
                return false;
                }
            catch (Exception e)
                {
                RaiseException(e);
                await TerminateAsync();
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
                    RaiseError("Попытка запуска завершенного объекта");
                    return false;
                    }

                if (IsRunning)
                    {
                    RaiseError("Попытка запуска выполняющегося объекта");
                    return false;
                    }

                if (RunOnlyOnce && HasBeenRun)
                    {
                    RaiseError("Повторный запуск запрещен");
                    return false;
                    }

                #region Выполнение

                using (var gt = new ActorDisposableTime($"выполнения '{Name}'", RaiseMessage))
                    {
                    if (!IsStarted)
                        {
                        bres = await StartAsync();
                        if (!bres)
                            {
                            return false;
                            }
                        }

                    m_ExecutionTime.SetStartDate();
                    SetActorState(ActorState.Running);

                    HasBeenRun = true;

                    var runtask = InternalRunAsync();
                    await (runtask);
                    m_ExecutionTime.SetEndDate();

                    if (IsStarted || IsRunning)
                        {
                        if (RunOnlyOnce)
                            {
                            bres = await StopAsync();
                            if (!bres)
                                {
                                await TerminateAsync();
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
                        bres = await TerminateAsync();
                        if (!bres)
                            {
                            return false;
                            }
                        }

                    return runtask.Result;
                    }

                #endregion Выполнение
                } // end try
            catch (AggregateException ae)
                {
                m_ExecutionTime.SetEndDate();

                foreach (Exception e in ae.InnerExceptions)
                    {
                    RaiseException(e);
                    }
                await TerminateAsync();
                return false;
                } // end catch
            catch (Exception e)
                {
                m_ExecutionTime.SetEndDate();

                RaiseException(e);
                await TerminateAsync();
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
                var bres = await RunCleanupBeforeTerminationAsync();
                return bres;
                }
            catch (Exception ex)
                {
                RaiseException(ex);
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