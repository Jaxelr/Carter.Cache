using System;
using Carter.Cache.Stores;
using Enyim.Caching;

namespace Carter.Cache.Memcached
{
    public class MemcachedStore : ICacheStore
    {
        private readonly IMemcachedClient client;

        public MemcachedStore(IMemcachedClient client) => this.client = client;

        /// <summary>
        /// Remove the element with the key provided.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) => client.Remove(key);

        /// <summary>
        /// Store the CachedResponse object and assign the key provided for the duration included.
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

            if (expiration.TotalSeconds > 0)
            {
                client.Set(key, response, expiration.Seconds);
            }
        }

        /// <summary>
        /// Try and get the value from the store and return it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="response"></param>
        /// <returns>True if the value exists, false if not</returns>
        public bool TryGetValue(string key, out CachedResponse response)
        {
            response = null;

            if (client.TryGet(key, out CachedResponse value))
            {
                response = value;
                return true;
            }

            return false;
        }
    }
}
