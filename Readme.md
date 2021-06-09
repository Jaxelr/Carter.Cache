# Carter.Cache [![Mit License][mit-img]][mit]

An extensible library to cache your [Carter][carter] modules.

## Builds

| Github  | Branch | Coverage |
| :---:     | :---: | :---: |
| ![.NET](https://github.com/Jaxelr/Carter.Cache/workflows/.NET/badge.svg?branch=master) | master | [![CodeCov][codecov-master-img]][codecov-master] |

## Packages

Package | NuGet (Stable) | MyGet (Prerelease)
| :--- | :---: | :---: |
| Carter.Cache | [![NuGet][carter-cache-img]][carter-cache] | [![MyGet][myget-carter-cache-img]][myget-carter-cache] |
| Carter.Cache.Memcached | [![NuGet][carter-cache-memcached-img]][carter-cache-memcached] | [![MyGet][myget-carter-cache-memcached-img]][myget-carter-cache-memcached] |

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

### Memcached store

A memcached store which includes the dependency on [EnyimMemcachedCore](https://www.nuget.org/packages/EnyimMemcachedCore/) and can be plugged in to replace the in memory store.
The usage requires the following reconfigurations on the ConfigureServices method of Startup:

```csharp
    public void ConfigureServices(IServiceCollection services)
        {
            //...

            //EnyimMemcached requires a logging 
            services.AddLogging(opt =>
            {
                opt.ClearProviders();
                opt.AddConsole();
                opt.AddDebug();
                opt.AddConfiguration(Configuration.GetSection("Logging"));
            });

            //Point to the server / port desired
            services.AddEnyimMemcached(options => options.AddServer("127.0.0.1", 11211));

            //Resolve the IMemcachedClient dependency using EnyimMemcached
            services.AddSingleton<ICacheStore>(provider => new MemcachedStore(provider.GetRequiredService<IMemcachedClient>()));

            //Define Caching options using the store configured
            services.AddSingleton(provider => new CachingOption() { Store = provider.GetRequiredService<ICacheStore>() });

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            //Pass it as a dependency to the add
            services.AddCarterCaching(serviceProvider.GetRequiredService<CachingOption>());

            //...
        }
```

For more information check the [sample](/samples/Sample.Carter.Cache.Memcached.Application) included.

[carter-cache-img]: https://img.shields.io/nuget/v/Carter.Cache.svg
[carter-cache]: https://www.nuget.org/packages/Carter.Cache
[myget-carter-cache-img]: https://img.shields.io/myget/carter-cache/v/Carter.Cache.svg
[myget-carter-cache]: https://www.myget.org/feed/carter-cache/package/nuget/Carter.Cache
[carter-cache-memcached-img]: https://img.shields.io/nuget/v/Carter.Cache.Memcached.svg
[carter-cache-memcached]: https://www.nuget.org/packages/Carter.Cache.Memcached
[myget-carter-cache-memcached-img]: https://img.shields.io/myget/carter-cache/v/Carter.Cache.Memcached.svg
[myget-carter-cache-memcached]: https://www.myget.org/feed/carter-cache/package/nuget/Carter.Cache.Memcached
[mit-img]: http://img.shields.io/badge/License-MIT-blue.svg
[mit]: https://github.com/Jaxelr/Carter.Cache/blob/master/LICENSE
[carter]: https://github.com/CarterCommunity/Carter
[codecov-master-img]: https://codecov.io/gh/Jaxelr/Carter.Cache/branch/master/graph/badge.svg
[codecov-master]: https://codecov.io/gh/Jaxelr/Carter.Cache/branch/master
