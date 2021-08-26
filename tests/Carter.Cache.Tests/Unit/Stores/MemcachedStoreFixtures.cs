using System;
using Carter.Cache.Memcached;
using Carter.Cache.Tests.Fakes;
using Enyim.Caching;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Carter.Cache.Tests.Unit.Stores
{
    public class MemcachedStoreFixtures
    {
        private const string Host = "127.0.0.1";
        private const int Port = 11211;

        private static MemcachedClient GetMemcachedClient()
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddEnyimMemcached(options => options.AddServer(Host, Port));
            services.AddLogging();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetService<IMemcachedClient>() as MemcachedClient;
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

            var response = A.Fake<FakeCachedResponse>();

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
            const string key = "Random-Key-1";

            var response = A.Fake<FakeCachedResponse>();

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
            const string key = "Random-Key-3";

            var response = A.Fake<FakeCachedResponse>();

            //Act
            cache.Set(key, response, expiration);
            cache.Remove(key);
            bool found = cache.TryGetValue(key, out var getResponse);

            //Assert
            Assert.Null(getResponse);
            Assert.False(found);
        }

        [Fact]
        public void Memcached_cache_with_value_set_get()
        {
            //Arrange
            var client = GetMemcachedClient();
            var cache = new MemcachedStore(client);
            var expiration = new TimeSpan(0, 1, 0);
            const string key = "Random-Key-4";
            const string contentType = "application/json";
            const int statusCode = 200;

            var response = new CachedResponse
            {
                ContentType = contentType,
                Expiry = expiration,
                StatusCode = statusCode,
                ContentLength = 1,
            };

            //Act
            cache.Set(key, response, expiration);
            bool found = cache.TryGetValue(key, out var getResponse);

            //Assert
            Assert.Equal(response.ContentType, getResponse.ContentType);
            Assert.Equal(response.StatusCode, getResponse.StatusCode);
            Assert.Equal(response.Expiry, getResponse.Expiry);
            Assert.Equal(1, response.ContentLength);
            Assert.True(found);
        }

        [Fact]
        public void Memcached_cache_empty_set_no_timespan()
        {
            //Arrange
            var client = GetMemcachedClient();
            var cache = new MemcachedStore(client);
            var expiration = new TimeSpan(0, 0, 0);
            const string key = "-Random-Key-5";

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
    }
}
