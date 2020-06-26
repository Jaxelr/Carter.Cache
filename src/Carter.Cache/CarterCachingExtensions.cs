using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Carter.Cache
{
    public static class CarterCachingExtensions
    {
        public static void AddCarterCaching(this IServiceCollection services, Action<CachingOptions> options = null)
        {
            options ??= _ => { };

            services.AddSingleton(typeof(CachingOptions), options);
        }

        public static void UseCarterCaching(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetService<CachingOptions>();

            builder.UseMiddleware<CarterCachingMiddleware>(options);
        }
    }
}
