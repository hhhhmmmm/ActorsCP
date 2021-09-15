using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorsCP.Actors
    {
    /// <summary>
    /// Очередь для последовательного выполнения
    /// </summary>
    public class ActorsQueue : ActorsSet
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorsQueue()
            {
            SetName("Очередь объектов");
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название очереди объектов</param>
        public ActorsQueue(string name) : base(name)
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

            var array = WaitingList.ToArray();
            foreach (var actor in array)
                {
                ThrowIfCancellationRequested();
                bool bres = await actor.RunAsync();
                if (!bres)
                    {
                    bresult = false;
                    }
                }

            return bresult;
            }

        #endregion Перегруженные методы
        } // end class ActorsQueue
    } // end namespace ActorsCP.Actors