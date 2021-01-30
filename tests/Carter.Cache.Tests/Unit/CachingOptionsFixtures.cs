using Carter.Cache.Keys;
using Carter.Cache.Stores;
using Xunit;

namespace Carter.Cache.Tests.Unit
{
    public class CachingOptionsFixtures
    {
        [Fact]
        public void Default_caching_option()
        {
            //Arrange & Act
            var cachingOption = new CachingOption();

            //Assert
            Assert.IsType<DefaultMemoryStore>(cachingOption.Store);
            Assert.IsType<DefaultKeyGenerator>(cachingOption.Key);
        }

        [Fact]
        public void Default_caching_option_with_constructor_key()
        {
            //Arrange
            var key = new DefaultKeyGenerator();

            //Act
            var cachingOption = new CachingOption(key);

            //Assert
            Assert.StrictEqual(key, cachingOption.Key);
        }

        [Fact]
        public void Default_caching_option_with_constructor_store()
        {
            //Arrange
            var store = new DefaultMemoryStore();

            //Act
            var cachingOption = new CachingOption(store);

            //Assert
            Assert.StrictEqual(store, cachingOption.Store);
        }


        [Fact]
        public void Default_caching_option_with_constructor_store_max_size()
        {
            //Arrange
            const long Size = 10;

            //Act
            var cachingOption = new CachingOption(Size);
            var store = (DefaultMemoryStore) cachingOption.Store;

            //Assert
            Assert.StrictEqual(Size, store.SizeLimit);
        }

        [Fact]
        public void Default_caching_option_with_constructor_key_and_store()
        {
            //Arrange
            var key = new DefaultKeyGenerator();
            var store = new DefaultMemoryStore();

            //Act
            var cachingOption = new CachingOption(key, store);

            //Assert
            Assert.StrictEqual(key, cachingOption.Key);
            Assert.StrictEqual(store, cachingOption.Store);
        }
    }
}
