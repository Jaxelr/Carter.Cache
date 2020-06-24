using System;

namespace Carter.Cache.Stores
{
    public interface ICacheStore
    {
        CachedResponse Get(string key);

        void Set(string key, CachedResponse response, TimeSpan expiration);

        void Remove(string key);
    }
}
