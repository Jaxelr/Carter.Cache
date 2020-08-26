using Carter.Cache.Stores;
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
            Assert.False(found);
            Assert.Null(response);
        }
    }
}
