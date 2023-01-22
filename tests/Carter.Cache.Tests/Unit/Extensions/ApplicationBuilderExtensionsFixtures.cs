using FakeItEasy;
using Microsoft.AspNetCore.Builder;
using Xunit;

namespace Carter.Cache.Tests.Unit.Extensions;

public class ApplicationBuilderExtensionsFixtures
{
    [Fact]
    public void Application_builder()
    {
        //Arrange
        var builder = A.Fake<IApplicationBuilder>();

        A.CallTo(() => builder.ApplicationServices.GetService(typeof(CachingOption)))
            .Returns(new CachingOption());

        //Act
        builder.UseCarterCaching();

        //Assert
        Assert.NotNull(builder.ServerFeatures.Get<CarterCachingService>());
    }
}
