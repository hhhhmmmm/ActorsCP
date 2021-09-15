using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

// https://www.dotnetperls.com/concurrentdictionary

namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Вспомогательный потокобезопасный класс-контейнер
    /// </summary>
    /// <typeparam name="TTypeparam">тип</typeparam>
    public sealed class ConcurrentContainerT<TTypeparam> where TTypeparam : class
        {
        /// <summary>
        /// Залипуха для контейнера
        /// </summary>
        private const byte DefaultValue = 0;

        #region Свойства

        /// <summary>
        /// Контейнер
        /// </summary>
        private ConcurrentDictionary<TTypeparam, byte> Bag
            {
            get;
            } = new ConcurrentDictionary<TTypeparam, byte>();

        /// <summary>
        /// Контейнер пуст
        /// </summary>
        public bool IsEmpty
            {
            get
                {
                return Bag.IsEmpty;
                }
            }

        /// <summary>
        /// Количество элементов
        /// </summary>
        public int Count
            {
            get
                {
                return Bag.Count;
                }
            }

        /// <summary>
        /// Ключи
        /// </summary>
        public ICollection<TTypeparam> Items
            {
            get
                {
                return Bag.Keys;
                }
            }

        #endregion Свойства

        #region Методы

        /// <summary>
        /// Добавить элемент в контейнер
        /// </summary>
        /// <param name="item">Объект</param>
        public void Add(TTypeparam item)
            {
            var bag = Bag;

            var bres = false;
            for (var i = 0; i < 100; i++)
                {
                bres = bag.TryAdd(item, DefaultValue);
                if (bres)
                    {
                    break;
                    }
                }
            if (!bres)
                {
                throw new InvalidOperationException("Не удалось добавить элемент в контейнер");
                }
            }

        /// <summary>
        /// Удалить элемент из контейнера
        /// </summary>
        /// <param name="item">Объект</param>
        public bool Remove(TTypeparam item)
            {
            IDictionary<TTypeparam, byte> id = Bag;
            var bres = id.Remove(item);
            return bres;
            }

        /// <summary>
        /// Содержится в контейнере
        /// </summary>
        /// <param name="item">Объект</param>
        /// <returns></returns>
        public bool Contains(TTypeparam item)
            {
            if (Bag.ContainsKey(item))
                {
                return true;
                }
            return false;
            }

        /// <summary>
        /// Очистить контейнер
        /// </summary>
        public void Clear()
            {
            Bag.Clear();
            }

        /// <summary>
        /// Список
        /// </summary>
        public IEnumerable<TTypeparam> List
            {
            get
                {
                return Bag.Keys;
                }
            }

        #endregion Методы
        }
    }