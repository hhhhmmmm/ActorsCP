namespace ItMobileApp.General.Utils
    {
        /// <summary>
        /// Шаблон синглтона
        /// </summary>
        /// <typeparam name="T">ссылочный объект с открытым конструктором по умолчанию.</typeparam>
        public abstract class Singleton<T> where T : class, new()
        {
        /// <summary>
        /// Единственный экзмепляр объекта
        /// </summary>
        private static T _instance;

        /// <summary>
        /// Получить экземпляр объекта
        /// </summary>
        /// <returns>единственный экземпляр</returns>
        public static T GetInstance()
            {
            if (_instance == null)
                {
                _instance = new T();
                }
            return _instance;
            }
        }
    }