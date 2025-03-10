# Carter.Cache [![Mit License][mit-img]][mit]

An extensible library to cache your [Carter][carter] modules.

## Builds

| Github  | Branch | Coverage |
| :---:     | :---: | :---: |
| Github Actions | [![.NET][github-master-img]][github-master] | master | [![CodeCov][codecov-master-img]][codecov-master] |

## Packages

Package | NuGet (Stable) | MyGet (Prerelease)
| :--- | :---: | :---: |
| Carter.Cache | [![NuGet][carter-cache-img]][carter-cache] | [![MyGet][myget-carter-cache-img]][myget-carter-cache] |
| Carter.Cache.Memcached | [![NuGet][carter-cache-memcached-img]][carter-cache-memcached] | [![MyGet][myget-carter-cache-memcached-img]][myget-carter-cache-memcached] |
| Carter.Cache.Redis | [![NuGet][carter-cache-redis-img]][carter-cache-redis] | [![MyGet][myget-carter-cache-redis-img]][myget-carter-cache-redis] |

## Installation

Install via [nuget][carter-cache]

```powershell
PM> Install-Package Carter.Cache
```

This library depends on Carter to properly work, you can install [Carter][carter] using the following command:

```powershell
PM> Install-Package Carter
```

## Sample usage

1. Add carter caching to your Program.cs:

```csharp
var builder = WebApplication.CreateBuilder(args);

//The rest of your Program.cs configuration ....

//It is recommended to always provide a caching max size limit
builder.Services.AddCarterCaching(new CachingOption(2048));
builder.Services.AddCarter();
```

1. Define a Configuration usage

```csharp
var app = builder.Build();

//The rest of your configuration usage ....

app.UseCarterCaching();
app.MapCarter();

app.Run();

```



1. Add the Cacheable clause to your module:

```csharp
    public class HomeModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", (HttpContext ctx) =>
            {
                ctx.AsCacheable(10); //In Seconds

                ctx.Response.StatusCode = 200;
                return ctx.Response.WriteAsync("Hello world");
            });
        }
    }
```

The default configuration does use the [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory) library as a default caching mechanism. _Note:_  By default memory caching can put lots of pressure on the memory of your system, please refer to the following [microsoft in-memory cache docs](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory) for basics on usage.

### Customization

You can easily define a custom Store by implementing the ICacheStore interface with the following signature:

```csharp
    public interface ICacheStore
    {
        bool TryGetValue(string key, out CachedResponse cachedResponse);

        void Set(string key, CachedResponse response, TimeSpan expiration);

        void Remove(string key);
    }
```

Also a custom Key can be easily defined by implementing the ICacheKey interface:

```csharp
    public interface ICacheKey
    {
        string Get(HttpRequest request);
    }
```

### Redis store

A redis store which includes the dependency on [StackExchange.Redis](https://www.nuget.org/packages/StackExchange.Redis/) and can be used as a replacement of the memory store.

Firstly, install the library using .net cli `dotnet add package Carter.Cache.Redis` or using Package Manager `Install-Package Carter.Cache.Redis`. The usage requires the following configurations on the Startup.cs file:

```csharp
//The rest of your Program.cs ....

builder.Services.AddSingleton<ICacheStore>(new RedisStore("127.0.0.1:6379"));
builder.Services.AddSingleton(provider => new CachingOption()
{
    Store = provider.GetRequiredService<ICacheStore>()
});

IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

builder.Services.AddCarterCaching(serviceProvider.GetRequiredService<CachingOption>());
builder.Services.AddCarter();
```

As part of the redis definition, you can optionally provide a json serializer definition to be used for the serialization of the cached response. This uses the [System.Text.Json](https://www.nuget.org/packages/System.Text.Json) library.

```csharp
//The rest of your Program.cs ....

var jsonOptions = new System.Text.Json.JsonSerializerOptions() { AllowTrailingCommas = true };

builder.Services.AddSingleton<ICacheStore>(new RedisStore("127.0.0.1:6379", jsonOptions));
builder.Services.AddSingleton(provider => new CachingOption()
{
    Store = provider.GetRequiredService<ICacheStore>()
});

IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

builder.Services.AddCarterCaching(serviceProvider.GetRequiredService<CachingOption>());
builder.Services.AddCarter();
```

### Memcached store

Alternatively, a memcached store can also be included as an alternatively, using a dependency on the library [EnyimMemcachedCore](https://www.nuget.org/packages/EnyimMemcachedCore/).

To install, using .net cli `dotnet add package Carter.Cache.Memcached` or using Package Manager `Install-Package Carter.Cache.Memcached`. The usage requires the following reconfigurations on the ConfigureServices method of Startup:

```csharp
//The rest of your Program.cs ....

//Point to the server / port desired
builder.Services.AddEnyimMemcached(options => options.AddServer("127.0.0.1", 11211));

//Resolve the IMemcachedClient dependency using EnyimMemcached
builder.Services.AddSingleton<ICacheStore>(provider => new MemcachedStore(provider.GetRequiredService<IMemcachedClient>()));

//Define Caching options using the store configured
builder.Services.AddSingleton(provider => new CachingOption()
{
    Store = provider.GetRequiredService<ICacheStore>()
});

IServiceProvider serviceProvider = services.BuildServiceProvider();

//Pass it as a dependency to the add
services.AddCarterCaching(serviceProvider.GetRequiredService<CachingOption>());
```

For more information check the [samples](/samples) included.

[carter-cache-img]: https://img.shields.io/nuget/v/Carter.Cache.svg
[carter-cache]: https://www.nuget.org/packages/Carter.Cache
[myget-carter-cache-img]: https://img.shields.io/myget/carter-cache/v/Carter.Cache.svg
[myget-carter-cache]: https://www.myget.org/feed/carter-cache/package/nuget/Carter.Cache
[carter-cache-memcached-img]: https://img.shields.io/nuget/v/Carter.Cache.Memcached.svg
[carter-cache-memcached]: https://www.nuget.org/packages/Carter.Cache.Memcached
[myget-carter-cache-memcached-img]: https://img.shields.io/myget/carter-cache/v/Carter.Cache.Memcached.svg
[myget-carter-cache-memcached]: https://www.myget.org/feed/carter-cache/package/nuget/Carter.Cache.Memcached
[carter-cache-redis-img]: https://img.shields.io/nuget/v/Carter.Cache.Redis.svg
[carter-cache-redis]: https://www.nuget.org/packages/Carter.Cache.Redis
[myget-carter-cache-redis-img]: https://img.shields.io/myget/carter-cache/v/Carter.Cache.Redis.svg
[myget-carter-cache-redis]: https://www.myget.org/feed/carter-cache/package/nuget/Carter.Cache.Redis
[mit-img]: http://img.shields.io/badge/License-MIT-blue.svg
[mit]: https://github.com/Jaxelr/Carter.Cache/blob/master/LICENSE
[carter]: https://github.com/CarterCommunity/Carter
[codecov-master-img]: https://codecov.io/gh/Jaxelr/Carter.Cache/branch/master/graph/badge.svg
[codecov-master]: https://codecov.io/gh/Jaxelr/Carter.Cache/branch/master
[github-master]: https://github.com/Jaxelr/Carter.Cache/actions/workflows/ci.yml
[github-master-img]: https://github.com/Jaxelr/Carter.Cache/actions/workflows/ci.yml/badge.svg
