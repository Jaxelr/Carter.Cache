using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
using Sample.Carter.Cache.Memcached.Application;
using Sample.Carter.Cache.Memcached.Application.Repository;
using Xunit;

namespace Carter.Cache.Sample.Memcached.Tests.Unit
{
    public class SampleAppMemcachedFixtures : IDisposable
    {
        private readonly HttpClient client;
        private readonly TestServer server;

        public SampleAppMemcachedFixtures()
        {
            var featureCollection = new FeatureCollection();
            featureCollection.Set<IServerAddressesFeature>(new ServerAddressesFeature());
            featureCollection.Set<IHelloRepository>(new Mocks.MockHelloRepository());

            server = new TestServer(WebHost.CreateDefaultBuilder()
                    .UseStartup<Startup>(), featureCollection
            );

            client = server.CreateClient();
        }

        public void Dispose()
        {
            server?.Dispose();
            client?.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task Hello_module_get_hello_world()
        {
            //Arrange
            const string name = "myMemcachedUser";

            //Act
            var res = await client.GetAsync($"/hello/{name}");

            //Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains(name, await res.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Hello_module_get_hello_world_from_cache()
        {
            //Arrange
            const string name = "myMemcachedUser2";

            //Act
            var res1 = await client.GetAsync($"/hello/{name}");
            var res2 = await client.GetAsync($"/hello/{name}");

            //Assert
            Assert.Equal(res2.StatusCode, res1.StatusCode);
            Assert.Equal(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
            Assert.True(res2.Headers.Contains("X-Carter-Cache-Expiration"));
        }

        [Fact]
        public async Task Hello_module_get_hello_world_from_cache_with_etag()
        {
            //Arrange
            const string name = "myMemcachedUser7";
            const string etag = "Etag";

            //Act
            var res1 = await client.GetAsync($"/hello/{name}");
            var res2 = await client.GetAsync($"/hello/{name}");

            //Assert
            Assert.Equal(res2.StatusCode, res1.StatusCode);
            Assert.Equal(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
            Assert.True(res1.Headers.Contains(etag));
            Assert.True(res2.Headers.Contains(etag));
            Assert.True(res2.Headers.Contains("X-Carter-Cache-Expiration"));
        }

        [Fact]
        public async Task Hello_module_get_hello_world_from_cache_expired()
        {
            //Arrange
            const string name = "myMemcachedUser3";

            //Act
            var res1 = await client.GetAsync($"/hello/{name}");
            Thread.Sleep(new TimeSpan(0, 0, 0, 10));
            var res2 = await client.GetAsync($"/hello/{name}");

            //Assert
            Assert.Equal(res2.StatusCode, res1.StatusCode);
            Assert.NotEqual(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
            Assert.False(res2.Headers.Contains("X-Carter-Cache-Expiration"));
        }

        [Fact]
        public async Task Hello_module_get_hello_world2()
        {
            //Arrange
            const string name = "myMemcachedUser4";

            //Act
            var res = await client.GetAsync($"/hello2/{name}");

            //Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains(name, await res.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Hello_module_get_hello_world2_from_cache()
        {
            //Arrange
            const string name = "myMemcachedUser5";

            //Act
            var res1 = await client.GetAsync($"/hello2/{name}");
            var res2 = await client.GetAsync($"/hello2/{name}");

            //Assert
            Assert.Equal(res2.StatusCode, res1.StatusCode);
            Assert.Equal(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
            Assert.True(res2.Headers.Contains("X-Carter-Cache-Expiration"));
        }

        [Fact]
        public async Task Hello_module_get_hello_world2_from_cache_expired()
        {
            //Arrange
            const string name = "myMemcachedUser6";

            //Act
            var res1 = await client.GetAsync($"/hello2/{name}");
            Thread.Sleep(new TimeSpan(0, 0, 0, 15));
            var res2 = await client.GetAsync($"/hello2/{name}");

            //Assert
            Assert.Equal(res2.StatusCode, res1.StatusCode);
            Assert.NotEqual(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
            Assert.False(res2.Headers.Contains("X-Carter-Cache-Expiration"));
        }
    }
}
