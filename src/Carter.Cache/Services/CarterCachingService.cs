using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public class CarterCachingService : ICarterCachingService
    {
        public async Task<bool> CheckCache(HttpContext ctx, CachingOption options)
        {
            string key = options.Key.Get(ctx.Request);

            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (options.Store.TryGetValue(key, out CachedResponse cachedResponse))
            {
                await cachedResponse.MapToContext(ctx);
                return true;
            }

            return false;
        }

        public async Task SetCache(HttpContext context, CachedResponse response, CachingOption options)
        {
            string key = options.Key.Get(context.Request);

            options.Store.Set(key, response, response.Expiry);

            await Task.CompletedTask;
        }
    }
}
