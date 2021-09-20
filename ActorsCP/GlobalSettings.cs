using System;
using System.Text;

using ActorsCP.Options;

namespace ActorsCP
    {
    /// <summary>
    /// Вспомогательный класс для глобальных настроек библиотеки
    /// </summary>
    public static class GlobalSettings
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        static GlobalSettings()
            {
            }

        #endregion Конструкторы

        #region Методы

        /// <summary>
        /// Добавить или обновить пару ключ/значение для всех объектов
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="newValue">Значение</param>
        public static void AddOrUpdateActorOption(string key, object newValue)
            {
            var gao = GlobalActorOptions.GetInstance();
            gao.AddOrUpdate(key, newValue);
            }

        /// <summary>
        /// Добавить или обновить пару ОТЛАДОЧНЫЙ ключ/значение для всех объектов
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="newValue">Значение</param>
        public static void AddOrUpdateActorDebugOption(string key, object newValue)
            {
            var gao = GlobalActorDebugOptions.GetInstance();
            gao.AddOrUpdate(key, newValue);
            }

        #endregion Методы
        } // end class GlobalSettings
    } // end namespace ActorsCP