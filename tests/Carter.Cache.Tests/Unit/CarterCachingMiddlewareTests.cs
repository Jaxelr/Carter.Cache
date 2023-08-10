using System;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Carter.Cache.Tests.Unit;

public class CarterCachingMiddlewareTests
{
    [Fact]
    public void Carter_caching_middleware_constructor()
    {
        //Arrange
        var reqDelegate = A.Fake<RequestDelegate>();
        var service = A.Fake<CarterCachingService>();
        var option = A.Fake<CachingOption>();

        //Act
        var middleware = new CarterCachingMiddleware(reqDelegate, service, option);

        //Assert
        Assert.NotNull(middleware);
    }

    [Fact]
    public async Task Carter_caching_middleware_invoke_with_invalid_request()
    {
        //Arrange
        const string scheme = "http";
        const string host = "localhost";
        const int port = 80;
        const string path = "/hello";
        const int elapsedSeconds = 3;

        var fakeSpan = TimeSpan.FromSeconds(elapsedSeconds);
        var ctx = A.Fake<HttpContext>();
        var option = A.Fake<CachingOption>();
        var response = A.Fake<HttpResponse>();
        var service = A.Fake<CarterCachingService>();
        var request = A.Fake<HttpRequest>();
        var props = A.Fake<CachingProperty>();
        var reqDelegate = A.Fake<RequestDelegate>();

        props.Expiration = TimeSpan.FromSeconds(elapsedSeconds);

        A.CallTo(() => ctx.Features.Get<CachingProperty>()).Returns(props);
        A.CallTo(() => ctx.Request).Returns(request);
        A.CallTo(() => ctx.Response).Returns(response);

        A.CallTo(() => request.Scheme).Returns(scheme);
        A.CallTo(() => request.Host).Returns(new HostString(host, port));
        A.CallTo(() => request.Path).Returns(path);

        //Act
        var middleware = new CarterCachingMiddleware(reqDelegate, service, option);
        await middleware.Invoke(ctx);

        //Assert
        Assert.NotNull(middleware);
    }

    [Fact]
    public async Task Carter_caching_middleware_invoke_with_valid_request()
    {
        //Arrange
        const string scheme = "http";
        const string host = "localhost";
        const int port = 80;
        const string path = "/hello";
        const string method = "GET";
        const int elapsedSeconds = 3;

        var fakeSpan = TimeSpan.FromSeconds(elapsedSeconds);
        var ctx = A.Fake<HttpContext>();
        var option = A.Fake<CachingOption>();
        var response = A.Fake<HttpResponse>();
        var service = A.Fake<CarterCachingService>();
        var request = A.Fake<HttpRequest>();
        var props = A.Fake<CachingProperty>();
        var reqDelegate = A.Fake<RequestDelegate>();
        var stream = A.Fake<MemoryStream>();

        props.Expiration = TimeSpan.FromSeconds(elapsedSeconds);

        A.CallTo(() => ctx.Features.Get<CachingProperty>()).Returns(props);
        A.CallTo(() => ctx.Request).Returns(request);
        A.CallTo(() => ctx.Response).Returns(response);

        A.CallTo(() => request.Scheme).Returns(scheme);
        A.CallTo(() => request.Host).Returns(new HostString(host, port));
        A.CallTo(() => request.Path).Returns(path);
        A.CallTo(() => request.Method).Returns(method);

        A.CallTo(() => response.StatusCode).Returns(StatusCodes.Status200OK);
        A.CallTo(() => response.Body).Returns(stream);
        A.CallTo(() => response.Body.Length).Returns(5);

        //Act
        var middleware = new CarterCachingMiddleware(reqDelegate, service, option);
        await middleware.Invoke(ctx);

        //Assert
        Assert.NotNull(middleware);
    }

    [Fact]
    public async Task Carter_caching_middleware_invoke_with_invalid_response()
    {
        //Arrange
        const string scheme = "http";
        const string host = "localhost";
        const int port = 80;
        const string path = "/hello400";
        const string method = "GET";
        const int elapsedSeconds = 3;

        var fakeSpan = TimeSpan.FromSeconds(elapsedSeconds);
        var ctx = A.Fake<HttpContext>();
        var option = A.Fake<CachingOption>();
        var response = A.Fake<HttpResponse>();
        var service = A.Fake<CarterCachingService>();
        var request = A.Fake<HttpRequest>();
        var props = A.Fake<CachingProperty>();
        var reqDelegate = A.Fake<RequestDelegate>();
        var stream = A.Fake<MemoryStream>();

        props.Expiration = TimeSpan.FromSeconds(elapsedSeconds);

        A.CallTo(() => ctx.Features.Get<CachingProperty>()).Returns(props);
        A.CallTo(() => ctx.Request).Returns(request);
        A.CallTo(() => ctx.Response).Returns(response);

        A.CallTo(() => request.Scheme).Returns(scheme);
        A.CallTo(() => request.Host).Returns(new HostString(host, port));
        A.CallTo(() => request.Path).Returns(path);
        A.CallTo(() => request.Method).Returns(method);

        A.CallTo(() => response.StatusCode).Returns(StatusCodes.Status400BadRequest);
        A.CallTo(() => response.Body).Returns(stream);
        A.CallTo(() => response.Body.Length).Returns(5);

        //Act
        var middleware = new CarterCachingMiddleware(reqDelegate, service, option);
        await middleware.Invoke(ctx);

        //Assert
        Assert.NotNull(middleware);
        Assert.False(ctx.Response.Headers.ContainsKey(HeaderNames.ETag));
    }

    [Fact]
    public async Task Carter_caching_middleware_invoke_multiple_with_valid_request()
    {
        //Arrange
        const string scheme = "http";
        const string host = "localhost";
        const int port = 80;
        const string path = "/hello";
        const string method = "GET";
        const int elapsedSeconds = 3;

        var fakeSpan = TimeSpan.FromSeconds(elapsedSeconds);
        var ctx = A.Fake<HttpContext>();
        var option = A.Fake<CachingOption>();
        var response = A.Fake<HttpResponse>();
        var service = A.Fake<CarterCachingService>();
        var request = A.Fake<HttpRequest>();
        var props = A.Fake<CachingProperty>();
        var reqDelegate = A.Fake<RequestDelegate>();
        var stream = A.Fake<MemoryStream>();

        props.Expiration = TimeSpan.FromSeconds(elapsedSeconds);

        A.CallTo(() => ctx.Features.Get<CachingProperty>()).Returns(props);
        A.CallTo(() => ctx.Request).Returns(request);
        A.CallTo(() => ctx.Response).Returns(response);

        A.CallTo(() => request.Scheme).Returns(scheme);
        A.CallTo(() => request.Host).Returns(new HostString(host, port));
        A.CallTo(() => request.Path).Returns(path);
        A.CallTo(() => request.Method).Returns(method);

        A.CallTo(() => response.StatusCode).Returns(StatusCodes.Status200OK);
        A.CallTo(() => response.Body).Returns(stream);
        A.CallTo(() => response.Body.Length).Returns(5);

        //Act
        var middleware = new CarterCachingMiddleware(reqDelegate, service, option);
        await middleware.Invoke(ctx);
        await middleware.Invoke(ctx);

        //Assert
        Assert.NotNull(middleware);
    }
}
