﻿using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Carter.Cache.Tests.Unit
{
    public class CachingServiceFxtures
    {
        [Fact]
        public async Task Service_check_cache_default()
        {
            //Arrange
            var ctx = A.Fake<HttpContext>();
            var option = A.Fake<CachingOption>();
            var service = A.Fake<CarterCachingService>();

            //Act
            bool cacheHit = await service.CheckCache(ctx, option);

            //Assert
            Assert.False(cacheHit);
        }

        [Fact]
        public async Task Service_check_cache_with_request()
        {
            //Arrange
            const string scheme = "http";
            const string host = "localhost";
            const int port = 80;
            const string path = "/hello";

            var ctx = A.Fake<HttpContext>();
            var option = A.Fake<CachingOption>();
            var service = A.Fake<CarterCachingService>();
            var request = A.Fake<HttpRequest>();

            A.CallTo(() => request.Scheme).Returns(scheme);
            A.CallTo(() => request.Host).Returns(new HostString(host, port));
            A.CallTo(() => request.Path).Returns(path);

            //Act
            bool cacheHit = await service.CheckCache(ctx, option);

            //Assert
            Assert.False(cacheHit);
        }

        [Fact]
        public async Task Service_set_cache_default()
        {
            //Arrange
            var ctx = A.Fake<HttpContext>();
            var option = A.Fake<CachingOption>();
            var response = A.Fake<CachedResponse>();
            var service = A.Fake<CarterCachingService>();

            //Act
            await service.SetCache(ctx, response, option);
            bool cacheHit = await service.CheckCache(ctx, option);

            //Assert
            Assert.False(cacheHit);
        }

        [Fact]
        public async Task Service_set_cache_default_with_request()
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

            props.Expiration = TimeSpan.FromSeconds(elapsedSeconds);

            A.CallTo(() => ctx.Features.Get<CachingProperty>()).Returns(props);
            A.CallTo(() => ctx.Request).Returns(request);
            A.CallTo(() => ctx.Response).Returns(response);

            A.CallTo(() => request.Scheme).Returns(scheme);
            A.CallTo(() => request.Host).Returns(new HostString(host, port));
            A.CallTo(() => request.Path).Returns(path);

            A.CallTo(() => response.StatusCode).Returns(StatusCodes.Status200OK);

            var cachedResponse = new CachedResponse(ctx, Array.Empty<byte>());

            //Act
            await service.SetCache(ctx, cachedResponse, option);
            bool cacheHit = await service.CheckCache(ctx, option);

            //Assert
            Assert.True(cacheHit);
        }
    }
}
