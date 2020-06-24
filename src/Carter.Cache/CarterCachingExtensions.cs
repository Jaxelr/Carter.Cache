using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Carter.Cache
{
    public static class CarterCachingExtensions
    {
        public static IServiceCollection AddCarterCaching(this IServiceCollection services, CachingOptions options) =>
            services.AddSingleton(typeof(CachingOptions), options);

        public static void UseCarterCaching(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetService<CachingOptions>();

            builder.UseMiddleware<CarterCachingMiddleware>(options);
        }
    }
}
