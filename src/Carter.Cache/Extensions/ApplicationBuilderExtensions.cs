using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Carter.Cache
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseCarterCaching(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetService<CachingOption>();

            builder.UseMiddleware<CarterCachingMiddleware>(options);
        }
    }
}
