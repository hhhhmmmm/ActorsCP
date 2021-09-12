using System;
using System.Collections.Concurrent;
using System.Text;

namespace ActorsCP.Options
    {
    // http://dir.by/developer/csharp/concurrent_dictionary/add_or_update/

    /// <summary>
    /// Опции объекта
    /// </summary>
    public class ActorOptions : IActorOptions
        {
        private readonly ConcurrentDictionary<string, object> _bag = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Получить объект
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Get(string key, out object value)
            {
            var bres = _bag.TryGetValue(key, out value);
            if (bres)
                {
                return true;
                }

            var gao = GlobalActorOptions.GetInstance();
            bres = gao.Get(key, out value);
            return bres;
            }

        public bool AddOrUpdate(string key, object value)
            {
            _bag.AddOrUpdate(key, value, (_key, oldValue) => value);
            return true;
            }
        }
    }