using System;
using Carter.Cache.Keys;
using Carter.Cache.Stores;

namespace Carter.Cache
{
    public class CachingOptions
    {
        public ICacheKey Key { get; set; }
        public ICacheStore Store { get; set; }
        public TimeSpan Expiry { get; set; }

        public CachingOptions() : this(new DefaultKeyGenerator(), new DefaultMemoryStore())
        {
        }

        public CachingOptions(long maxSize) : this(new DefaultKeyGenerator(), new DefaultMemoryStore(maxSize))
        {
        }

        public CachingOptions(ICacheKey key) : this(key, new DefaultMemoryStore())
        {
        }

        public CachingOptions(ICacheStore store) : this(new DefaultKeyGenerator(), store)
        {
        }

        public CachingOptions(ICacheKey key, ICacheStore store) => (Key, Store) = (key, store);
    }
}
