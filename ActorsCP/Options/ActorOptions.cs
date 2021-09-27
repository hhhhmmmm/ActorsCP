using System;
using System.Collections.Concurrent;

namespace ActorsCP.Options
    {
    /// <summary>
    /// Опции объекта
    /// </summary>
    public class ActorOptions : IActorOptions
        {
        /// <summary>
        /// Контейнер
        /// </summary>
        private readonly ConcurrentDictionary<string, object> _bag = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Добавить или обновить пару ключ/значение
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="newValue">Значение</param>
        public void AddOrUpdate(string key, object newValue)
            {
            if (string.IsNullOrEmpty(key))
                {
                throw new ArgumentNullException(nameof(key), "Значение key не может быть null или пустой строкой");
                }
            _bag.AddOrUpdate(key, newValue, (_key, oldValue) => newValue);
            }

        /// <summary>
        /// Получить объект по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="value">Значение</param>
        /// <returns>true если удалось извлечь значение</returns>
        public bool Get(string key, out object value)
            {
            var bres = _bag.TryGetValue(key, out value);
            if (bres)
                {
                return true;
                }

            var gao = GlobalActorOptions.GetInstance();

            if (gao == this) // обращение к глобальному контейнеру
                {
                return false;
                }

            bres = gao.Get(key, out value);
            return bres;
            }

        /// <summary>
        /// Получить объект типа bool по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="boolValue">Значение</param>
        /// <returns>true если удалось извлечь значение</returns>
        public bool GetBool(string key, out bool boolValue)
            {
            boolValue = false;

            bool bres;
            object value;
            bres = Get(key, out value);
            if (!bres)
                {
                return false;
                }

            if (value is bool b)
                {
                boolValue = b;
                return true;
                }

            return false;
            }

        /// <summary>
        /// Получить объект типа int по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="intValue">Значение</param>
        /// <returns>true если удалось извлечь значение</returns>
        public bool GetInt(string key, out int intValue)
            {
            intValue = 0;

            bool bres;
            object value;
            bres = Get(key, out value);
            if (!bres)
                {
                return false;
                }

            if (value is int _int)
                {
                intValue = _int;
                return true;
                }

            if (value is Int16 _int16)
                {
                intValue = _int16;
                return true;
                }

            if (value is bool b)
                {
                intValue = b ? 1 : 0;
                return true;
                }

            if (value is string s)
                {
                bres = int.TryParse(s, out intValue);
                return bres;
                }

            return false;
            }

        /// <summary>
        /// Получить объект типа int по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="stringValue">Значение</param>
        /// <returns>true если удалось извлечь значение</returns>
        public bool GetString(string key, out string stringValue)
            {
            stringValue = null;

            bool bres;
            object value;
            bres = Get(key, out value);
            if (!bres)
                {
                return false;
                }

            if (value == null)
                {
                return true;
                }

            if (value is string s)
                {
                stringValue = s;
                return true;
                }

            var so = value.ToString();
            stringValue = so;

            return true;
            }
        }
    }