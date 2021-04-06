using System;
using Carter.Cache.Stores;

namespace Carter.Cache.Memcached
{
    public class MemcachedStore : ICacheStore
    {
        public void Remove(string key) => throw new NotImplementedException();
        public void Set(string key, CachedResponse response, TimeSpan expiration) => throw new NotImplementedException();
        public bool TryGetValue(string key, out CachedResponse cachedResponse) => throw new NotImplementedException();
    }
}
