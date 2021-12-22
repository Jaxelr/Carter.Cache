using System;
using Microsoft.Extensions.DependencyInjection;

namespace Carter.Cache;

public static class ServiceCollectionExtensions
{
    public static void AddCarterCaching(this IServiceCollection services, Action<CachingOption> options = null)
    {
        var cachingOptions = new CachingOption();

        options(cachingOptions);

        services.AddCarterCaching(cachingOptions);
    }

    public static void AddCarterCaching(this IServiceCollection services, CachingOption cachingOptions)
    {
        services.AddSingleton(cachingOptions);
        services.AddSingleton<ICarterCachingService, CarterCachingService>();
    }

    public static void AddCarterCaching(this IServiceCollection services) => services.AddCarterCaching(new CachingOption());
}
