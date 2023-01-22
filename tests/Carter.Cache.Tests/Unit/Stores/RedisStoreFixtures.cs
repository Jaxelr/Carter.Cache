using System;
using Carter.Cache.Redis;
using Carter.Cache.Tests.Fakes;
using FakeItEasy;
using StackExchange.Redis;
using Xunit;

namespace Carter.Cache.Tests.Unit.Stores;

public class RedisStoreFixtures
{
    private const string REDIS_LOCALHOST = "127.0.0.1:6379";

    [Fact]
    public void Redis_cache_empty_get()
    {
        //Arrange
        var cache = new RedisStore(REDIS_LOCALHOST);

        //Act
        bool found = cache.TryGetValue(string.Empty, out var response);

        //Assert
        Assert.NotNull(cache);
        Assert.False(found);
        Assert.Null(response);
    }

    [Fact]
    public void Redis_cache_empty_get_with_ConfigOptions()
    {
        //Arrange
        var cache = new RedisStore(ConfigurationOptions.Parse(REDIS_LOCALHOST));

        //Act
        bool found = cache.TryGetValue(string.Empty, out var response);

        //Assert
        Assert.NotNull(cache);
        Assert.False(found);
        Assert.Null(response);
    }

    [Fact]
    public void Redis_cache_empty_remove()
    {
        //Arrange
        var cache = new RedisStore(REDIS_LOCALHOST);

        //Act
        cache.Remove(string.Empty);

        //Assert
        Assert.NotNull(cache);
    }

    [Fact]
    public void Redis_cache_empty_key()
    {
        //Arrange
        var cache = new RedisStore(REDIS_LOCALHOST);
        var expiration = new TimeSpan(0, 1, 0);

        var response = A.Fake<FakeCachedResponse>();

        //Act
        cache.Set(string.Empty, response, expiration);

        //Assert
        Assert.NotNull(cache);
        Assert.NotNull(response);
    }

    [Fact]
    public void Redis_cache_empty_set()
    {
        //Arrange
        var cache = new RedisStore(REDIS_LOCALHOST);
        var expiration = new TimeSpan(0, 1, 0);
        const string key = "-Random-Key-";

        var response = A.Fake<FakeCachedResponse>();

        //Act
        cache.Set(key, response, expiration);

        //Assert
        Assert.NotNull(cache);
        Assert.NotNull(response);
    }

    [Fact]
    public void Redis_cache_empty_set_no_timespan()
    {
        //Arrange
        var cache = new RedisStore(REDIS_LOCALHOST);
        var expiration = new TimeSpan(0, 0, 0);
        const string key = "-Random-Key-1";

        var response = A.Fake<FakeCachedResponse>();

        //Act
        cache.Set(key, response, expiration);
        bool found = cache.TryGetValue(key, out var getResponse);

        //Assert
        Assert.NotNull(cache);
        Assert.NotNull(response);
        Assert.False(found);
        Assert.Null(getResponse);
    }

    [Fact]
    public void Redis_cache_with_value_set_get()
    {
        //Arrange
        var cache = new RedisStore(REDIS_LOCALHOST);
        var expiration = new TimeSpan(0, 1, 0);
        const string key = "-Random-Key-2";

        var response = A.Fake<FakeCachedResponse>();

        //Act
        cache.Set(key, response, expiration);
        bool found = cache.TryGetValue(key, out var getResponse);

        //Assert
        Assert.True(found);
        Assert.Equal(response.ContentType, getResponse.ContentType);
        Assert.Equal(response.Body, getResponse.Body);
        Assert.Equal(response.ContentLength, getResponse.ContentLength);
        Assert.Equal(response.StatusCode, getResponse.StatusCode);
        Assert.Equal(response.Expiry, getResponse.Expiry);
        Assert.Equal(response.Headers, getResponse.Headers);
    }

    [Fact]
    public void Redis_cache_with_value_set_get_with_size_limit()
    {
        //Arrange
        var cache = new RedisStore(REDIS_LOCALHOST);
        var expiration = new TimeSpan(0, 1, 0);
        const string key = "-Random-Key-3";

        var response = A.Fake<FakeCachedResponse>();

        //Act
        cache.Set(key, response, expiration);
        bool found = cache.TryGetValue(key, out var getResponse);

        //Assert
        Assert.True(found);
        Assert.Equal(response.ContentType, getResponse.ContentType);
        Assert.Equal(response.Body, getResponse.Body);
        Assert.Equal(response.ContentLength, getResponse.ContentLength);
        Assert.Equal(response.StatusCode, getResponse.StatusCode);
        Assert.Equal(response.Expiry, getResponse.Expiry);
        Assert.Equal(response.Headers, getResponse.Headers);
    }

    [Fact]
    public void Redis_cache_with_value_set_remove_get()
    {
        //Arrange
        var cache = new RedisStore(REDIS_LOCALHOST);
        var expiration = new TimeSpan(0, 1, 0);
        const string key = "-Random-Key-4";

        var response = A.Fake<FakeCachedResponse>();

        //Act
        cache.Set(key, response, expiration);
        cache.Remove(key);
        bool found = cache.TryGetValue(key, out var getResponse);

        //Assert
        Assert.Null(getResponse);
        Assert.False(found);
    }
}
