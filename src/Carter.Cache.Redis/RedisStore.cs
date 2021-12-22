using System;
using Carter.Cache.Stores;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Carter.Cache.Redis;

public class RedisStore : ICacheStore
{
    private readonly ConnectionMultiplexer redis;

    public RedisStore(ConfigurationOptions options)
    {
        redis = ConnectionMultiplexer.Connect(options);
    }

    public RedisStore(string configuration)
    {
        redis = ConnectionMultiplexer.Connect(configuration);
    }

    public void Dispose() => redis.Dispose();

    /// <summary>
    /// Remove the element with the key provided.
    /// </summary>
    /// <param name="key"></param>
    public void Remove(string key)
    {
        var cache = redis.GetDatabase();
        cache.KeyDelete(key);
    }

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
            var cache = redis.GetDatabase();

            bool ack = cache.StringSet(key, JsonConvert.SerializeObject(response), expiry: expiration);

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

        var cache = redis.GetDatabase();

        var result = cache.StringGet(key);

        if (result.HasValue)
        {
            response = JsonConvert.DeserializeObject<CachedResponse>(result);
            return true;
        }

        return false;
    }
}
