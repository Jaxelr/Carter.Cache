using System;
using Microsoft.Extensions.DependencyInjection;

namespace Carter.Cache
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCarterCaching(this IServiceCollection services, Action<CachingOptions> options = null)
        {
            var cachingOptions = new CachingOptions();

            options(cachingOptions);

            services.AddCarterCaching(cachingOptions);
        }

        public static void AddCarterCaching(this IServiceCollection services, CachingOptions cachingOptions)
        {
            services.AddSingleton(cachingOptions);
            services.AddSingleton<ICarterCachingService, CarterCachingService>();
        }

        public static void AddCarterCaching(this IServiceCollection services)
        {
            services.AddCarterCaching(new CachingOptions());
        }
    }
}
