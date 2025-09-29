using FakeItEasy;
using Microsoft.AspNetCore.Builder;

namespace Carter.Cache.Tests.Unit.Extensions;

public class ApplicationBuilderExtensionsTests
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
