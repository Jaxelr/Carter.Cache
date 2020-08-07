using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Carter.Cache
{
    public static class CarterCachingExtensions
    {
        public static void AddCarterCaching(this IServiceCollection services, Action<CachingOptions> options = null)
        {
            var cachingOptions = new CachingOptions();

            options(cachingOptions);

            services.AddSingleton(typeof(CachingOptions), cachingOptions);
            services.AddSingleton<ICarterCachingService, CarterCachingService>();
        }

        public static void UseCarterCaching(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetService<CachingOptions>();

            builder.UseMiddleware<CarterCachingMiddleware>(options);
        }
    }
}
