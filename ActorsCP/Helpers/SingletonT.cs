namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Шаблон синглтона
    /// </summary>
    /// <typeparam name="T">ссылочный объект с открытым конструктором по умолчанию.</typeparam>
    public abstract class SingletonT<T> where T : class, new()
        {
        /// <summary>
        /// Единственный экзмепляр объекта
        /// </summary>
        private static T _instance;

        /// <summary>
        /// Локер
        /// </summary>
        private static readonly object _locker = new object();

        /// <summary>
        /// Получить экземпляр объекта
        /// </summary>
        /// <returns>единственный экземпляр</returns>
        public static T GetInstance()
            {
            lock (_locker)
                {
                if (_instance == null)
                    {
                    _instance = new T();
                    }
                return _instance;
                }
            }
        }
    }