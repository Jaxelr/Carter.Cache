using Carter.Cache.Keys;
using Xunit;

namespace Carter.Cache.Tests.Unit.Keys
{
    public class DefaultKeyGeneratorFixtures
    {
        [Fact]
        public void Default_keyed_null()
        {
            //Arrange
            var keyGen = new DefaultKeyGenerator();

            //Act
            string key = keyGen.Get(null);

            //Assert
            Assert.Empty(key);
        }
    }
}
