using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace RnCache
{
    public class MemCacheRnCache : IRnCache
    {
        private readonly MemoryCache _cache = MemoryCache.Default; // they swear it's thread safe
        public CacheItemPolicy CachePolicy = new CacheItemPolicy() {SlidingExpiration = new TimeSpan(0, 0, 60, 0)};

        private readonly Dictionary<string, object> _funcDictionary = new Dictionary<string, object>();
        private readonly object _fdlock = new object();

        public void InitEntityCache<T>(string entityId, Func<T> entityRetriever, bool lazy = false)
        {
            if (String.IsNullOrEmpty(entityId))
                return;
            lock (_fdlock)
            {
                if (_funcDictionary.ContainsKey(entityId))
                    _funcDictionary.Remove(entityId);
                _funcDictionary[entityId] = entityRetriever;
            }
            if (!lazy)
            {
                UpdateEntityCache(entityId, forceSyncUpdate: true);
            }
        }

        public T GetCachedEntity<T>(string entityId, bool updateIfNull = true, bool forceUpdate = false)
        {
            if (String.IsNullOrEmpty(entityId))
                return default(T);
            var cachedEntities = _cache.Get(entityId);
            if ((cachedEntities == null && updateIfNull) || forceUpdate)
            {
                UpdateEntityCache(entityId, forceSyncUpdate: true);
                cachedEntities = _cache.Get(entityId);
            }
            if (!(cachedEntities is T))
            {
                return default(T);
            }
            return (T) cachedEntities;
        }


        public Task UpdateEntityCache(string entityId, bool forceSyncUpdate = false)
        {
            var nothing = Task.Factory.StartNew(() => { });
            if (String.IsNullOrEmpty(entityId))
                return nothing;
            Func<object> entityRetriever;

            lock (_fdlock)
            {
                if (!_funcDictionary.ContainsKey(entityId))
                    return nothing;
                var funcObj = _funcDictionary[entityId];
                if (!(funcObj is Func<object>))
                    return nothing;
                entityRetriever = (Func<object>) funcObj;
            }
            var cachedEntities = _cache.Get(entityId);
            Action update = () =>
            {
                var entities = entityRetriever();
                _cache.Set(entityId, entities, CachePolicy);
            };
            var t = Task.Factory.StartNew(update); // async update
            if (cachedEntities == null || forceSyncUpdate) //  wait if we need to
            {
                t.Wait();
            }
            return t;
        }
    }
}