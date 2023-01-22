using System.Linq;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Carter.Cache.Tests.Unit.Extensions;

public class ServiceCollectionExtensionsFixtures
{
    [Fact]
    public void Service_collection()
    {
        //Arrange
        var service = A.Fake<IServiceCollection>();

        //Act
        service.AddCarterCaching();

        //Assert
        Assert.NotNull(service.Where(x => x.ServiceType == typeof(CachingOption)));
    }

    [Fact]
    public void Service_collection_with_parameter()
    {
        //Arrange
        var service = A.Fake<IServiceCollection>();
        var options = new CachingOption(2048);

        //Act
        service.AddCarterCaching(options);

        //Assert
        Assert.NotNull(service.Where(x => x.ServiceType == typeof(CachingOption)));
    }

    [Fact]
    public void Service_collection_with_action_parameter()
    {
        //Arrange
        var service = A.Fake<IServiceCollection>();

        //Act
        service.AddCarterCaching(options => options.ValidRequest = null);

        //Assert
        Assert.NotNull(service.Where(x => x.ServiceType == typeof(CachingOption)));
    }
}
