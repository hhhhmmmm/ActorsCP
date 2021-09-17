using System;
using System.Collections.Generic;
using System.Text;

namespace ActorsCP.Options
    {
    /// <summary>
    /// Интерфейс опций объекта
    /// </summary>
    public interface IActorOptions
        {
        /// <summary>
        /// Добавить или обновить пару ключ/значение
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="newValue">Значение</param>
        void AddOrUpdate(string key, object newValue);

        /// <summary>
        /// Получить объект по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="value">Значение</param>
        /// <returns>true если удалось извлечь значение</returns>
        bool Get(string key, out object value);

        /// <summary>
        /// Получить объект типа bool по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="boolValue">Значение</param>
        /// <returns>true если удалось извлечь значение</returns>
        bool GetBool(string key, out bool boolValue);

        /// <summary>
        /// Получить объект типа int по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="intValue">Значение</param>
        /// <returns>true если удалось извлечь значение</returns>
        bool GetInt(string key, out int intValue);

        /// <summary>
        /// Получить объект типа int по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="stringValue">Значение</param>
        /// <returns>true если удалось извлечь значение</returns>
        bool GetString(string key, out string stringValue);
        }
    }