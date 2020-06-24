using System;
using Microsoft.Extensions.Caching.Memory;

namespace Carter.Cache.Stores
{
    public class DefaultMemoryStore : ICacheStore
    {
        private readonly IMemoryCache cache;
        private readonly long sizeLimit;
        private long size = 0;

        public DefaultMemoryStore() : this(new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public DefaultMemoryStore(MemoryCacheOptions options) : this(new MemoryCache(options))
        {
            sizeLimit = options.SizeLimit ?? 0;
        }

        public DefaultMemoryStore(int maxSize) : this(new MemoryCacheOptions() { SizeLimit = maxSize })
        {
        }

        public DefaultMemoryStore(MemoryCache cache) => (this.cache, size) = (cache, 0);

        public CachedResponse Get(string key)
        {
            cache.TryGetValue(key, out CachedResponse value);

            return value;
        }

        public void Remove(string key) => cache.Remove(key);

        public void Set(string key, CachedResponse response, TimeSpan expiration)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (sizeLimit > 0 && sizeLimit == size && !ContainsKey(key))
            {
                return;
            }

            if (expiration.TotalSeconds > 0)
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(expiration)
                    .RegisterPostEvictionCallback(callback: Eviction, state: this)
                    .SetSize(sizeLimit);

                cache.Set(key, response, options);
                size++;
            }
        }

        /// <summary>
        /// Verify the cache contains the key indicated.
        /// </summary>
        /// <param name="key"></param>
        private bool ContainsKey(string key) => cache.TryGetValue(key, out _);

        /// <summary>
        /// Once the record is evicted from the cache, subtract 1 from  the size
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="reason"></param>
        /// <param name="state"></param>
        private void Eviction(object key, object value, EvictionReason reason, object state) => size--;
    }
}
