using System;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Carter.Cache.Tests.Unit.Extensions
{
    public class HttpContextExtensionsFixtures
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
    }
}
