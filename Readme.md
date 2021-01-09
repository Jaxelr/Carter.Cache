# Carter.Cache [![Mit License][mit-img]][mit]

An extensable library to cache your [Carter][carter] modules.

## Builds

| Github  | Branch |
| :---:     | :---: |
| ![.NET](https://github.com/Jaxelr/Carter.Cache/workflows/.NET/badge.svg?branch=master) | master |

## Packages

Package | NuGet (Stable) | MyGet (Prerelease)
| :--- | :---: | :---: |
| Carter.Cache | [![NuGet][carter-cache-img]][carter-cache] | [![MyGet][myget-carter-cache-img]][myget-carter-cache] |

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

1. Add carter caching to your Startup.cs:

```csharp
using Carter.Cache;

// The rest of your Startup.cs definition...

    public void ConfigureServices(IServiceCollection services)
        {
            //It is recommended to always provide a caching max size limit
            services.AddCarterCaching(new CachingOption(2048)); 
            services.AddCarter();
        }
```

1. Define a Configuration usage

```csharp
    public void Configure(IApplicationBuilder app, AppSettings appSettings)
    {
        app.UseCarterCaching();
        app.UseEndpoints(builder => builder.MapCarter());
    }
```

1. Add the Cacheable clause to your module:

```csharp
    public class HomeModule : CarterModule
    {
        public HomeModule()
        {
            Get("/", (req, res) =>
            {
                req.AsCacheable(10); //In Seconds

                res.StatusCode = 200;
                return res.WriteAsync("Hello world");
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

[carter-cache-img]: https://img.shields.io/nuget/v/Carter.Cache.svg
[carter-cache]: https://www.nuget.org/packages/Carter.Cache
[myget-carter-cache-img]: https://img.shields.io/myget/carter-cache/v/Carter.Cache.svg
[myget-carter-cache]: https://www.myget.org/feed/carter-cache/package/nuget/Carter.Cache
[mit-img]: http://img.shields.io/badge/License-MIT-blue.svg
[mit]: https://github.com/Jaxelr/Carter.Cache/blob/master/LICENSE
[carter]: https://github.com/CarterCommunity/Carter
