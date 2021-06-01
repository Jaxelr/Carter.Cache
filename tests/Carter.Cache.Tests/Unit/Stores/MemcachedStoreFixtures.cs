using System;
using Carter.Cache.Memcached;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Carter.Cache.Tests.Unit.Stores
{
    public class MemcachedStoreFixtures
    {
        private const string Host = "127.0.0.1";
        private const int Port = 11211;

        private static MemcachedClient GetMemcachedClient()
        {
            var factory = A.Fake<ILoggerFactory>();
            var configuration = A.Fake<MemcachedClientConfiguration>();

            configuration.AddServer(Host, Port);

            return new MemcachedClient(factory, configuration);
        }

        [Fact]
        public void Memcached_empty_get()
        {
            //Arrange
            var client = GetMemcachedClient();
            var cache = new MemcachedStore(client);

            //Act
            bool found = cache.TryGetValue(string.Empty, out var response);

            //Assert
            Assert.NotNull(cache);
            Assert.False(found);
            Assert.Null(response);
        }

        [Fact]
        public void Memcached_cache_empty_remove()
        {
            //Arrange
            var client = GetMemcachedClient();
            var cache = new MemcachedStore(client);

            //Act
            cache.Remove(string.Empty);

            //Assert
            Assert.NotNull(cache);
        }

        [Fact]
        public void Memcached_cache_empty_key()
        {
            //Arrange
            var client = GetMemcachedClient();
            var cache = new MemcachedStore(client);
            var expiration = new TimeSpan(0, 1, 0);

            var response = A.Fake<CachedResponse>();

            //Act
            cache.Set(string.Empty, response, expiration);

            //Assert
            Assert.NotNull(cache);
            Assert.NotNull(response);
        }

        [Fact]
        public void Memcached_cache_empty_set()
        {
            //Arrange
            var client = GetMemcachedClient();
            var cache = new MemcachedStore(client);
            var expiration = new TimeSpan(0, 1, 0);
            const string key = "-Random-Key-";

            var response = A.Fake<CachedResponse>();

            //Act
            cache.Set(key, response, expiration);

            //Assert
            Assert.NotNull(cache);
            Assert.NotNull(response);
        }

        [Fact]
        public void Memcached_cache_with_value_set_remove_get()
        {
            //Arrange
            var client = GetMemcachedClient();
            var cache = new MemcachedStore(client);
            var expiration = new TimeSpan(0, 1, 0);
            const string key = "-Random-Key-3";

            var response = A.Fake<CachedResponse>();

            //Act
            cache.Set(key, response, expiration);
            cache.Remove(key);
            bool found = cache.TryGetValue(key, out var getResponse);

            //Assert
            Assert.Null(getResponse);
            Assert.False(found);
        }
    }
}
