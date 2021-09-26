using System;
using System.Text;

namespace ActorsCP.Actors.Events
    {
    /// <summary>
    /// Событие - изменилось состояние набора объектов ActorSet
    /// </summary>
    public class ActorSetCountChangedEventArgs : ActorStateChangedEventArgs
        {
        /// <summary>
        /// Количество объектов ожидающих выполнения
        /// </summary>
        private readonly int _waitingCount;

        /// <summary>
        /// Количество выполняющихся объектов
        /// </summary>
        private readonly int _runningCount;

        /// <summary>
        /// Количество завершенных объектов
        /// </summary>
        private readonly int _completedCount;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="waitingCount">Количество объектов ожидающих выполнения</param>
        /// <param name="runningCount">Количество выполняющихся объектов</param>
        /// <param name="completedCount">Количество завершенных объектов</param>
        /// <param name="state"></param>
        public ActorSetCountChangedEventArgs(int waitingCount, int runningCount, int completedCount, ActorState state) : base(state)
            {
            _waitingCount = waitingCount;
            _runningCount = runningCount;
            _completedCount = completedCount;
            }

        #region Свойства

        /// <summary>
        /// Количество объектов ожидающих выполнения
        /// </summary>
        public int WaitingCount
            {
            get
                {
                return _waitingCount;
                }
            }

        /// <summary>
        ///  Количество выполняющихся объектов
        /// </summary>
        public int RunningCount
            {
            get
                {
                return _runningCount;
                }
            }

        /// <summary>
        ///  Количество завершенных объектов
        /// </summary>
        public int CompletedCount
            {
            get
                {
                return _completedCount;
                }
            }

        /// <summary>
        /// Общее количество объектов
        /// </summary>
        public int TotalCount
            {
            get
                {
                return _waitingCount + _runningCount + _completedCount;
                }
            }

        #endregion Свойства

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            {
            return $"Waiting: {WaitingCount}, Running={RunningCount}, Completed={CompletedCount}, Total = {TotalCount}, ActorState = {base.State}";
            }
        }
    }