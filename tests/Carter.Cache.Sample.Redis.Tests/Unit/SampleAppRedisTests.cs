﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Carter.Cache.Sample.Redis.Tests.Mocks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sample.Carter.Cache.Redis.Application.Repository;
using Xunit;

namespace Carter.Cache.Sample.Redis.Tests.Unit;

public class SampleAppRedisTests : IDisposable
{
    private readonly HttpClient client;
    private const string DefaultCacheHeader = "X-Carter-Cache-Expiration";

    public SampleAppRedisTests()
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
        const string name = "myRedisUser";

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
        const string name = "myRedisUser2";

        //Act
        var res1 = await client.GetAsync($"/hello/{name}");
        var res2 = await client.GetAsync($"/hello/{name}");

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.Equal(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
        Assert.True(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world_from_cache_expired()
    {
        //Arrange
        const string name = "myRedisUser3";

        //Act
        var res1 = await client.GetAsync($"/hello/{name}");
        Thread.Sleep(new TimeSpan(0, 0, 0, 10));
        var res2 = await client.GetAsync($"/hello/{name}");

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.NotEqual(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
        Assert.False(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world2()
    {
        //Arrange
        const string name = "myRedisUser4";

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
        const string name = "myRedisUser5";

        //Act
        var res1 = await client.GetAsync($"/hello2/{name}");
        var res2 = await client.GetAsync($"/hello2/{name}");

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.Equal(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
        Assert.True(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world2_from_cache_expired()
    {
        //Arrange
        const string name = "myRedisUser6";

        //Act
        var res1 = await client.GetAsync($"/hello2/{name}");
        Thread.Sleep(new TimeSpan(0, 0, 0, 15));
        var res2 = await client.GetAsync($"/hello2/{name}");

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.NotEqual(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
        Assert.False(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world_from_cache_with_etag()
    {
        //Arrange
        const string name = "myRedisUser7";
        const string etag = "Etag";

        //Act
        var res1 = await client.GetAsync($"/hello/{name}");
        var res2 = await client.GetAsync($"/hello/{name}");

        //Assert
        Assert.Equal(res2.StatusCode, res1.StatusCode);
        Assert.Equal(await res1.Content.ReadAsStringAsync(), await res2.Content.ReadAsStringAsync());
        Assert.True(res1.Headers.Contains(etag));
        Assert.True(res2.Headers.Contains(etag));
        Assert.True(res2.Headers.Contains(DefaultCacheHeader));
    }

    [Fact]
    public async Task Hello_module_get_hello_world_from_cache_with_etag_not_modified()
    {
        //Arrange
        const string name = "myRedisUser8";

        //Act
        var res1 = await client.GetAsync($"/hello/{name}");
        var etag = new EntityTagHeaderValue(res1.Headers.ETag.Tag);

        client.DefaultRequestHeaders.IfNoneMatch.Add(etag);
        var res2 = await client.GetAsync($"/hello/{name}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, res1.StatusCode);
        Assert.Equal(HttpStatusCode.NotModified, res2.StatusCode);
    }
}
