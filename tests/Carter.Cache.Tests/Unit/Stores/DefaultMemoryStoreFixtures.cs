using System;
using Carter.Cache.Stores;
using Carter.Cache.Tests.Fakes;
using FakeItEasy;
using Xunit;

namespace Carter.Cache.Tests.Unit.Stores
{
    public class DefaultMemoryStoreFixtures
    {
        [Fact]
        public void IMemory_cache_empty_get()
        {
            //Arrange
            var cache = new DefaultMemoryStore();

            //Act
            bool found = cache.TryGetValue(string.Empty, out var response);

            //Assert
            Assert.NotNull(cache);
            Assert.False(found);
            Assert.Null(response);
        }

        [Fact]
        public void IMemory_cache_empty_remove()
        {
            //Arrange
            var cache = new DefaultMemoryStore();

            //Act
            cache.Remove(string.Empty);

            //Assert
            Assert.NotNull(cache);
        }

        [Fact]
        public void IMemory_cache_empty_key()
        {
            //Arrange
            var cache = new DefaultMemoryStore();
            var expiration = new TimeSpan(0, 1, 0);

            var response = A.Fake<FakeCachedResponse>();

            //Act
            cache.Set(string.Empty, response, expiration);

            //Assert
            Assert.NotNull(cache);
            Assert.NotNull(response);
        }

        [Fact]
        public void IMemory_cache_empty_set()
        {
            //Arrange
            var cache = new DefaultMemoryStore();
            var expiration = new TimeSpan(0, 1, 0);
            const string key = "-Random-Key-1";

            var response = A.Fake<FakeCachedResponse>();

            //Act
            cache.Set(key, response, expiration);

            //Assert
            Assert.NotNull(cache);
            Assert.NotNull(response);
        }

        [Fact]
        public void IMemory_cache_with_value_set_get()
        {
            //Arrange
            var cache = new DefaultMemoryStore();
            var expiration = new TimeSpan(0, 1, 0);
            const string key = "-Random-Key-2";

            var response = A.Fake<FakeCachedResponse>();

            //Act
            cache.Set(key, response, expiration);
            bool found = cache.TryGetValue(key, out var getResponse);

            //Assert
            Assert.Equal(response, getResponse);
            Assert.True(found);
        }

        [Fact]
        public void IMemory_cache_with_value_set_get_with_size_limit()
        {
            //Arrange
            var cache = new DefaultMemoryStore(1);
            var expiration = new TimeSpan(0, 1, 0);
            const string key = "-Random-Key-3";

            var response = A.Fake<FakeCachedResponse>();

            //Act
            cache.Set(key, response, expiration);
            bool found = cache.TryGetValue(key, out var getResponse);

            //Assert
            Assert.Equal(response, getResponse);
            Assert.True(found);
        }

        [Fact]
        public void IMemory_cache_with_value_set_get_with_over_size_limit()
        {
            //Arrange
            var cache = new DefaultMemoryStore(1);
            var expiration = new TimeSpan(0, 1, 0);
            const string key = "-Random-Key-4";
            const string key2 = "-Random-Key-5";

            var response = A.Fake<FakeCachedResponse>();

            //Act
            cache.Set(key, response, expiration);
            cache.Set(key2, response, expiration);
            bool found = cache.TryGetValue(key, out var getResponse);

            //Assert
            Assert.Equal(response, getResponse);
            Assert.True(found);
        }

        [Fact]
        public void IMemory_cache_with_value_set_get_with_over_size_limit_same_key()
        {
            //Arrange
            var cache = new DefaultMemoryStore(1);
            var expiration = new TimeSpan(0, 1, 0);
            const string key = "-Random-Key-6";

            var response = A.Fake<FakeCachedResponse>();

            //Act
            cache.Set(key, response, expiration);
            cache.Set(key, response, expiration);
            bool found = cache.TryGetValue(key, out var getResponse);

            //Assert
            Assert.Equal(response, getResponse);
            Assert.True(found);
        }

        [Fact]
        public void IMemory_cache_with_value_set_remove_get()
        {
            //Arrange
            var cache = new DefaultMemoryStore();
            var expiration = new TimeSpan(0, 1, 0);
            const string key = "-Random-Key-7";

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
}
