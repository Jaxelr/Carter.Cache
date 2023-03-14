using System;
using System.Text.Json;
using Carter.Cache.Stores;
using StackExchange.Redis;

namespace Carter.Cache.Redis;

public class RedisStore : ICacheStore
{
    private readonly ConnectionMultiplexer redis;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public RedisStore(ConfigurationOptions options, JsonSerializerOptions jsonSerializerOptions = default)
    {
        redis = ConnectionMultiplexer.Connect(options);
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    public RedisStore(string configuration, JsonSerializerOptions jsonSerializerOptions = default)
    {
        redis = ConnectionMultiplexer.Connect(configuration);
        this.jsonSerializerOptions = jsonSerializerOptions;
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

            bool ack = cache.StringSet(key, JsonSerializer.Serialize(response, jsonSerializerOptions), expiry: expiration);

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
            response = JsonSerializer.Deserialize<CachedResponse>(result, jsonSerializerOptions);
            return true;
        }

        return false;
    }
}
