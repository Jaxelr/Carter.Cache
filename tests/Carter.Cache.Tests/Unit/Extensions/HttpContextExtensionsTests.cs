using System;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Carter.Cache.Tests.Unit.Extensions;

public class HttpContextExtensionsTests
{
    [Fact]
    public void HttpContext_as_cacheable_capability_int_seconds()
    {
        //Arrange
        const int elapsedSeconds = 3;

        var req = A.Fake<HttpContext>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.Features.Get<CachingProperty>()).Returns(props);

        //Act
        req.AsCacheable(elapsedSeconds);

        //Assert
        Assert.Equal(TimeSpan.FromSeconds(elapsedSeconds),
            req.Features.Get<CachingProperty>().Expiration);
    }

    [Fact]
    public void HttpContext_as_cacheable_capability_int_seconds_with_custom_header()
    {
        //Arrange
        const int elapsedSeconds = 3;
        const string fakeHeader = "X-FakeHeader";

        var req = A.Fake<HttpContext>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.Features.Get<CachingProperty>()).Returns(props);

        //Act
        req.AsCacheable(elapsedSeconds, fakeHeader);

        //Assert
        Assert.Equal(TimeSpan.FromSeconds(elapsedSeconds),
            req.Features.Get<CachingProperty>().Expiration);
        Assert.Equal(fakeHeader,
            req.Features.Get<CachingProperty>().CustomHeader);
    }

    [Fact]
    public void HttpContext_as_cacheable_capability_timespan_seconds()
    {
        //Arrange
        const int elapsedSeconds = 3;
        var fakeSpan = TimeSpan.FromSeconds(elapsedSeconds);

        var req = A.Fake<HttpContext>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.Features.Get<CachingProperty>()).Returns(props);

        //Act
        req.AsCacheable(fakeSpan);

        //Assert
        Assert.Equal(fakeSpan,
            req.Features.Get<CachingProperty>().Expiration);
    }

    [Fact]
    public void HttpContext_as_cacheable_capability_absolute_expiration()
    {
        //Arrange
        var fakeExpiration = DateTime.UtcNow.AddMinutes(1);
        var fakeSpan = fakeExpiration - DateTime.UtcNow;

        var req = A.Fake<HttpContext>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.Features.Get<CachingProperty>()).Returns(props);

        //Act
        req.AsCacheable(fakeExpiration);

        //Assert
        Assert.True(
            Math.Abs(fakeSpan.TotalMinutes - req.Features.Get<CachingProperty>().Expiration.TotalMinutes) < 0.01
        );
    }

    [Fact]
    public void HttpContext_as_cacheable_capability_timespan_seconds_with_custom_header()
    {
        //Arrange
        const int elapsedSeconds = 3;
        const string fakeHeader = "X-FakeHeader";
        var fakeSpan = TimeSpan.FromSeconds(elapsedSeconds);

        var req = A.Fake<HttpContext>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.Features.Get<CachingProperty>()).Returns(props);

        //Act
        req.AsCacheable(fakeSpan, fakeHeader);

        //Assert
        Assert.Equal(fakeSpan,
            req.Features.Get<CachingProperty>().Expiration);
        Assert.Equal(fakeHeader,
            req.Features.Get<CachingProperty>().CustomHeader);
    }

    [Fact]
    public void HttpContext_add_etag_with_invalid_response()
    {
        //Arrange
        const string checksum = "78a67136326368e9ee4879da01b81e2e1878ebfe";

        var req = A.Fake<HttpContext>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.Features.Get<CachingProperty>()).Returns(props);
        A.CallTo(() => req.Response.Headers.ContainsKey(HeaderNames.ETag)).Returns(false);
        A.CallTo(() => req.Response.StatusCode).Returns(StatusCodes.Status400BadRequest); /* Bad Request */

        //Act
        req.AddEtagToContext(checksum);

        //Assert
        Assert.False(req.Response.Headers.ContainsKey(HeaderNames.ETag));
    }

    [Fact]
    public void HttpContext_add_etag_with_valid_response()
    {
        //Arrange
        const string checksum = "78a67136326368e9ee4879da01b81e2e1878ebfe";

        var req = A.Fake<HttpContext>();
        var props = A.Fake<CachingProperty>();

        A.CallTo(() => req.Features.Get<CachingProperty>()).Returns(props);
        A.CallTo(() => req.Response.Headers.ContainsKey(HeaderNames.ETag)).Returns(false);
        A.CallTo(() => req.Response.StatusCode).Returns(StatusCodes.Status200OK); /* Ok */

        //Act
        req.AddEtagToContext(checksum);

        //Assert
        Assert.False(req.Response.Headers.ContainsKey(HeaderNames.ETag));
    }
}
