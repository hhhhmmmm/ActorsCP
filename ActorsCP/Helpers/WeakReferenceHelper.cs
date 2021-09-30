using System;
using System.Collections.Generic;
using System.Text;

namespace ActorsCP.Helpers
    {
    /// <summary>
    /// Вспомогательный класс для работы со слабыми ссылками
    /// </summary>
    public static class WeakReferenceHelper
        {
        /// <summary>
        /// Удалить все слабые ссылки на объект из списка
        /// </summary>
        /// <param name="list">Список</param>
        /// <param name="obj">Ссылка на объект</param>
        public static void DeleteWeakReference(List<WeakReference> list, object obj)
            {
            int foundItem;

            for (; ; )
                {
                foundItem = FindWeakReference(list, obj);
                if (foundItem == -1)
                    {
                    return;
                    }
                list.RemoveAt(foundItem);
                }
            }

        /// <summary>
        /// Найти слабую ссылку
        /// </summary>
        /// <param name="list">Список</param>
        /// <param name="obj">Ссылка на объект</param>
        /// <returns></returns>
        public static int FindWeakReference(List<WeakReference> list, object obj)
            {
            int foundItem = -1;

            for (int i = 0; i < list.Count; i++)
                {
                var weakRef = list[i];

                if (obj == weakRef.Target)
                    {
                    foundItem = i;
                    }
                }
            return foundItem;
            }
        } // end class WeakReferenceHelper
    } // end namespace ActorsCP.Helpers