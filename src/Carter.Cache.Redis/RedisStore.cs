using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Carter.Cache.Stores;
using StackExchange.Redis;

namespace Carter.Cache.Redis
{
    public class RedisStore : ICacheStore
    {
        private readonly ConnectionMultiplexer redis;
        private readonly IDatabase cache;

        public RedisStore(ConfigurationOptions options)
        {
            redis = ConnectionMultiplexer.Connect(options);
            cache = redis.GetDatabase();
        }

        public RedisStore(string configuration)
        {
            redis = ConnectionMultiplexer.Connect(configuration);
            cache = redis.GetDatabase();
        }

        /// <summary>
        /// Remove the element with the key provided.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) => cache.KeyDelete(key);

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
                bool ack = cache.StringSet(key, Serialize(response), expiry: expiration);

                if (!ack)
                {
                    throw new Exception($"Could not complete operation of Set, using redis configuration: {redis.Configuration}");
                }
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

            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var result = cache.StringGet(key);

            if (result.HasValue)
            {
                response = Deserialize<CachedResponse>(result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Serialize the object into an array of bytes using the binary formatter
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static byte[] Serialize(object o)
        {
            byte[] objectDataAsStream = null;

            if (o != null)
            {
                var binaryFormatter = new BinaryFormatter();
                using var memoryStream = new MemoryStream();
                binaryFormatter.Serialize(memoryStream, o);
                objectDataAsStream = memoryStream.ToArray();
            }

            return objectDataAsStream;
        }

        /// <summary>
        /// Deserialize the array of bytes into a poco using the binary formatter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static T Deserialize<T>(byte[] stream)
        {
            var result = default(T);

            if (stream != null)
            {
                var binaryFormatter = new BinaryFormatter();
                using var memoryStream = new MemoryStream(stream);
                result = (T) binaryFormatter.Deserialize(memoryStream);
            }

            return result;
        }
    }
}
