using System;
using Microsoft.Extensions.Caching.Memory;

namespace Carter.Cache.Stores
{
    public class DefaultMemoryStore : ICacheStore
    {
        private readonly IMemoryCache cache;
        internal readonly long SizeLimit;
        private long size;

        public DefaultMemoryStore() : this(new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public DefaultMemoryStore(MemoryCacheOptions options) : this(new MemoryCache(options))
        {
            SizeLimit = options.SizeLimit ?? 0;
        }

        public DefaultMemoryStore(long maxSize) : this(new MemoryCacheOptions() { SizeLimit = maxSize })
        {
        }

        public DefaultMemoryStore(MemoryCache cache) => (this.cache, size) = (cache, 0);

        /// <summary>
        /// Tries to get the value from the store and returns it, if so.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="response"></param>
        /// <returns>True if the value exists, false if it doesnt</returns>
        public bool TryGetValue(string key, out CachedResponse response)
        {
            response = null;

            if (cache.TryGetValue(key, out CachedResponse value))
            {
                response = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove the element with the key provided.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) => cache.Remove(key);

        /// <summary>
        /// Upsert the CachedResponse object and ties it to the key provided for the durantion of the expiration provided.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="response"></param>
        /// <param name="expiration"></param>
        public void Set(string key, CachedResponse response, TimeSpan expiration)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (SizeLimit > 0 && SizeLimit == size)
            {
                return;
            }

            if (expiration.TotalSeconds > 0)
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(expiration)
                    .RegisterPostEvictionCallback(callback: Eviction, state: this)
                    .SetSize(SizeLimit);

                cache.Set(key, response, options);
                size++;
            }
        }

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
