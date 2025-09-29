using System;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Carter.Cache.Tests.Unit.Models;

public class CachedResponseTests
{
    [Fact]
    public void Cached_response_maps()
    {
        //Arrange
        var context = A.Fake<HttpContext>();
        var props = new CachingProperty();

        //Act
        var cachedResponse = new CachedResponse(context, Array.Empty<byte>());

        //Assert
        Assert.True(cachedResponse.Headers.ContainsKey(props.CustomHeader));
    }

    [Fact]
    public void Cached_response_maps_null_property()
    {
        //Arrange
        var context = A.Fake<HttpContext>();
        byte[] input = Encoding.UTF8.GetBytes("Hello");
        A.CallTo(() => context.Features.Get<CachingProperty>()).Returns(null);

        //Act
        var cachedResponse = new CachedResponse(context, input);

        //Assert
        Assert.Null(cachedResponse.Body);
    }

    [Fact]
    public void Cached_response_maps_expiry()
    {
        //Arrange
        var timeSpan = new TimeSpan(0, 0, 5);
        var context = A.Fake<HttpContext>();
        var props = A.Fake<CachingProperty>();

        props.Expiration = timeSpan;
        A.CallTo(() => context.Features.Get<CachingProperty>()).Returns(props);

        //Act
        var cachedResponse = new CachedResponse(context, Array.Empty<byte>());

        //Assert
        Assert.Equal(timeSpan, cachedResponse.Expiry);
    }

    [Fact]
    public void Cached_response_maps_body()
    {
        //Arrange
        var context = A.Fake<HttpContext>();
        byte[] input = Encoding.UTF8.GetBytes("Hello");

        //Act
        var cachedResponse = new CachedResponse(context, input);

        //Assert
        Assert.Equal(cachedResponse.Body, input);
    }

    [Fact]
    public void Cached_response_maps_content_type()
    {
        //Arrange
        var context = A.Fake<HttpContext>();
        var response = A.Fake<HttpResponse>();

        A.CallTo(() => response.ContentType).Returns("application/json");
        A.CallTo(() => context.Response).Returns(response);

        //Act
        var cachedResponse = new CachedResponse(context, Array.Empty<byte>());

        //Assert
        Assert.Equal(cachedResponse.ContentType, response.ContentType);
    }

    [Fact]
    public void Cached_response_maps_status_code()
    {
        //Arrange
        var context = A.Fake<HttpContext>();
        var response = A.Fake<HttpResponse>();

        A.CallTo(() => response.StatusCode).Returns(200);
        A.CallTo(() => context.Response).Returns(response);

        //Act
        var cachedResponse = new CachedResponse(context, Array.Empty<byte>());

        //Assert
        Assert.Equal(cachedResponse.StatusCode, response.StatusCode);
    }

    [Fact]
    public async Task Http_context_maps_status_code()
    {
        //Arrange
        var context = A.Fake<HttpContext>();
        var response = A.Fake<HttpResponse>();

        A.CallTo(() => response.StatusCode).Returns(200);
        A.CallTo(() => context.Response).Returns(response);

        //Act
        var cachedResponse = new CachedResponse(context, Array.Empty<byte>());
        await cachedResponse.MapToContext(context);

        //Assert
        Assert.Equal(cachedResponse.StatusCode, response.StatusCode);
    }

    [Fact]
    public async Task Http_context_maps_content_type()
    {
        //Arrange
        var context = A.Fake<HttpContext>();
        var response = A.Fake<HttpResponse>();

        A.CallTo(() => response.ContentType).Returns("application/json");
        A.CallTo(() => context.Response).Returns(response);

        //Act
        var cachedResponse = new CachedResponse(context, Array.Empty<byte>());
        await cachedResponse.MapToContext(context);

        //Assert
        Assert.Equal(cachedResponse.ContentType, response.ContentType);
    }

    [Fact]
    public async Task Http_context_maps_etag()
    {
        //Arrange
        const string fakeEtag = "123456";
        var context = A.Fake<HttpContext>();
        var response = A.Fake<HttpResponse>();
        var request = A.Fake<HttpRequest>();

        A.CallTo(() => request.Headers.ContainsKey(HeaderNames.ETag)).Returns(true);
        A.CallTo(() => request.Headers[HeaderNames.ETag]).Returns(fakeEtag);

        A.CallTo(() => response.Headers.ContainsKey(HeaderNames.ETag)).Returns(true);
        A.CallTo(() => response.Headers[HeaderNames.ETag]).Returns(fakeEtag);
        A.CallTo(() => context.Request).Returns(request);
        A.CallTo(() => context.Response).Returns(response);

        //Act
        var cachedResponse = new CachedResponse(context, Array.Empty<byte>());
        await cachedResponse.MapToContext(context);

        //Assert
        Assert.Equal(cachedResponse.Headers[HeaderNames.ETag], response.Headers[HeaderNames.ETag]);
    }

    [Fact]
    public async Task Http_context_maps_if_none_match()
    {
        //Arrange
        const string fakeEtag = "84de625db71b56d480d47bdc32377d23144b8c65";
        var fakeIfNoneMatch = new Microsoft.Extensions.Primitives.StringValues(fakeEtag);
        var context = A.Fake<HttpContext>();
        var response = A.Fake<HttpResponse>();
        var request = A.Fake<HttpRequest>();

        A.CallTo(() => request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out fakeIfNoneMatch))
            .Returns(true);
        A.CallTo(() => context.Request).Returns(request);

        A.CallTo(() => response.Headers.ContainsKey(HeaderNames.ETag)).Returns(true);
        A.CallTo(() => response.Headers[HeaderNames.ETag]).Returns(fakeEtag);
        A.CallTo(() => context.Response).Returns(response);

        //Act
        var cachedResponse = new CachedResponse(context, Array.Empty<byte>());
        await cachedResponse.MapToContext(context);

        //Assert
        Assert.Equal(cachedResponse.Headers[HeaderNames.ETag], response.Headers[HeaderNames.ETag]);
        Assert.Equal(StatusCodes.Status304NotModified, response.StatusCode);
        Assert.Equal(0, response.ContentLength);
    }
}
