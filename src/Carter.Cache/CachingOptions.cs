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

        public CachingOptions(ICacheKey key, ICacheStore store) => (Key, Store) = (key, store);
    }
}
