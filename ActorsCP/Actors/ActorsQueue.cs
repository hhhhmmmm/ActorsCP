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
        /// Генератор имени
        /// </summary>
        private string NameGenerator
            {
            get
                {
                return "Очередь_" + ABN; // остальные - медленно
                }
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ActorsQueue()
            {
            SetName(NameGenerator);
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parentActor">Родительский объект</param>
        public ActorsQueue(ActorBase parentActor) : this(null, parentActor)
            {
            SetName(NameGenerator);
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название объекта</param>
        public ActorsQueue(string name) : this(name, null)
            {
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название объекта</param>
        /// <param name="parentActor">Родительский объект</param>
        public ActorsQueue(string name, ActorBase parentActor) : base(name, parentActor)
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

            RaiseActorsSetChanged();
            foreach (var actor in array)
                {
                ThrowIfCancellationRequested();
                bool bres = await actor.RunAsync().ConfigureAwait(false);
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