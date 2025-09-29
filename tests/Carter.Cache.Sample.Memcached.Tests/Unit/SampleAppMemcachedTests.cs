using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Carter.Cache.Sample.Memcached.Tests.Mocks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sample.Carter.Cache.Memcached.Application.Repository;

namespace Carter.Cache.Sample.Memcached.Tests.Unit;

public class SampleAppMemcachedTests : IDisposable
{
    private readonly HttpClient client;
    private const string DefaultCacheHeader = "X-Carter-Cache-Expiration";

    public SampleAppMemcachedTests()
    {
        var server = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => builder.ConfigureServices(services => services.AddSingleton<IHelloRepository, MockHelloRepository>()));

        client = server.CreateClient();
    }

    public void Dispose()
    {
        client?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Hello_module_get_hello_world()
    {
        //Arrange
        const string name = "myMemcachedUser";

        //Act
        var res = await client.GetAsync($"/hello/{name}", TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.Contains(name, await res.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Hello_module_get_hello_world_from_cache()
    {
        //Arrange
        const string name = "myMemcachedUser2";

        //Act
        var res1 = await client.GetAsync($"/hello/{name}", TestContext.Current.CancellationToken);
        var res2 = await client.GetAsync($"/hello/{name}", TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.Equal(await res1.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), await res2.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.True(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world_from_cache_expired()
    {
        //Arrange
        const string name = "myMemcachedUser3";

        //Act
        var res1 = await client.GetAsync($"/hello/{name}", TestContext.Current.CancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(15), TestContext.Current.CancellationToken);
        var res2 = await client.GetAsync($"/hello/{name}", TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.NotEqual(await res1.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), await res2.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.False(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world2()
    {
        //Arrange
        const string name = "myMemcachedUser4";

        //Act
        var res = await client.GetAsync($"/hello2/{name}", TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.Contains(name, await res.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Hello_module_get_hello_world2_from_cache()
    {
        //Arrange
        const string name = "myMemcachedUser5";

        //Act
        var res1 = await client.GetAsync($"/hello2/{name}", TestContext.Current.CancellationToken);
        var res2 = await client.GetAsync($"/hello2/{name}", TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.Equal(await res1.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), await res2.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.True(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world2_from_cache_expired()
    {
        //Arrange
        const string name = "myMemcachedUser6";

        //Act
        var res1 = await client.GetAsync($"/hello2/{name}", TestContext.Current.CancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(15), TestContext.Current.CancellationToken);
        var res2 = await client.GetAsync($"/hello2/{name}", TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.NotEqual(await res1.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), await res2.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.False(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world_from_cache_with_etag()
    {
        //Arrange
        const string name = "myMemcachedUser7";
        const string etag = "Etag";

        //Act
        var res1 = await client.GetAsync($"/hello/{name}", TestContext.Current.CancellationToken);
        var res2 = await client.GetAsync($"/hello/{name}", TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.Equal(await res1.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), await res2.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.True(res1.Headers.Contains(etag));
        Assert.True(res2.Headers.Contains(etag));
        Assert.True(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world_from_cache_with_etag_not_modified()
    {
        //Arrange
        const string name = "myMemcachedUser7";

        //Act
        var res1 = await client.GetAsync($"/hello/{name}", TestContext.Current.CancellationToken);
        var etag = new EntityTagHeaderValue(res1.Headers.ETag.Tag);

        client.DefaultRequestHeaders.IfNoneMatch.Add(etag);
        var res2 = await client.GetAsync($"/hello/{name}", TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(HttpStatusCode.OK, res1.StatusCode);
        Assert.Equal(HttpStatusCode.NotModified, res2.StatusCode);
    }
}
