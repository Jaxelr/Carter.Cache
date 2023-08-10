using System;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Carter.Cache.Tests.Unit.Extensions;

public class HttpRequestExtensionsTests
{
    [Fact]
    public void HttpRequest_as_cacheable_capability_int_seconds()
    {
        //Arrange
        const int elapsedSeconds = 3;

        var req = A.Fake<HttpRequest>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.HttpContext.Features.Get<CachingProperty>()).Returns(props);

        //Act
        req.AsCacheable(elapsedSeconds);

        //Assert
        Assert.Equal(TimeSpan.FromSeconds(elapsedSeconds),
            req.HttpContext.Features.Get<CachingProperty>().Expiration);
    }

    [Fact]
    public void HttpRequest_as_cacheable_capability_timespan_seconds()
    {
        //Arrange
        const int elapsedSeconds = 3;
        var fakeSpan = TimeSpan.FromSeconds(elapsedSeconds);

        var req = A.Fake<HttpRequest>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.HttpContext.Features.Get<CachingProperty>()).Returns(props);

        //Act
        req.AsCacheable(fakeSpan);

        //Assert
        Assert.NotNull(req);
        Assert.Equal(fakeSpan,
            req.HttpContext.Features.Get<CachingProperty>().Expiration);
    }

    [Fact]
    public void HttpRequest_as_cacheable_capability_int_seconds_and_custom_header()
    {
        //Arrange
        const int elapsedSeconds = 3;
        const string fakeHeader = "X-FakeHeader";

        var req = A.Fake<HttpRequest>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.HttpContext.Features.Get<CachingProperty>()).Returns(props);

        //Act
        req.AsCacheable(elapsedSeconds, fakeHeader);

        //Assert
        Assert.NotNull(req);
        Assert.Equal(TimeSpan.FromSeconds(elapsedSeconds),
            req.HttpContext.Features.Get<CachingProperty>().Expiration);
        Assert.Equal(fakeHeader,
            req.HttpContext.Features.Get<CachingProperty>().CustomHeader);
    }

    [Fact]
    public void HttpRequest_as_cacheable_capability_timespan_seconds_and_custom_header()
    {
        //Arrange
        const int elapsedSeconds = 3;
        const string fakeHeader = "X-FakeHeader";
        var fakeSpan = TimeSpan.FromSeconds(elapsedSeconds);

        var req = A.Fake<HttpRequest>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.HttpContext.Features.Get<CachingProperty>()).Returns(props);

        //Act
        req.AsCacheable(fakeSpan, fakeHeader);

        //Assert
        Assert.NotNull(req);
        Assert.Equal(fakeSpan,
            req.HttpContext.Features.Get<CachingProperty>().Expiration);
        Assert.Equal(fakeHeader,
            req.HttpContext.Features.Get<CachingProperty>().CustomHeader);
    }

    [Fact]
    public void HttpRequest_as_cacheable_capability_datetime_seconds_and_custom_header()
    {
        //Arrange
        const int elapsedSeconds = 3;
        const string fakeHeader = "X-FakeHeader";
        var fakeDate = DateTime.Now.AddSeconds(elapsedSeconds);

        var req = A.Fake<HttpRequest>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.HttpContext.Features.Get<CachingProperty>()).Returns(props);

        //Act
        var fakeSpanCalculated = fakeDate - DateTime.UtcNow;
        req.AsCacheable(fakeDate, fakeHeader);

        //Assert
        Assert.NotNull(req);
        Assert.True(fakeSpanCalculated.TotalSeconds - req.HttpContext.Features.Get<CachingProperty>().Expiration.TotalSeconds < 0.001); //Close Enough?
        Assert.Equal(fakeHeader, req.HttpContext.Features.Get<CachingProperty>().CustomHeader);
    }
}
