using System;

namespace Carter.Cache.Stores
{
    public interface ICacheStore : IDisposable
    {
        bool TryGetValue(string key, out CachedResponse cachedResponse);

        void Set(string key, CachedResponse response, TimeSpan expiration);

        void Remove(string key);
    }
}
