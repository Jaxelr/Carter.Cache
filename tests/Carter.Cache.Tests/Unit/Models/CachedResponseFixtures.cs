using System;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Carter.Cache.Tests.Unit.Models
{
    public class CachedResponseFixtures
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
    }
}
