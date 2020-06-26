using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public class CarterCachingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly CachingOptions options;

        public CarterCachingMiddleware(RequestDelegate next, CachingOptions options) => (this.next, this.options) = (next, options);

        public async Task Invoke(HttpContext context)
        {
            bool cacheHit = await CheckCache(context, options);

            if (!cacheHit)
            {
                await SetCache(context, next, options);
            }
        }

        private async Task SetCache(HttpContext context, RequestDelegate next, CachingOptions options)
        {
            string key = options.Key.Get(context.Request);

            HttpResponse response = context.Response;
            Stream originalStream = response.Body;

            try
            {
                using var memoryStream = new MemoryStream();
                response.Body = memoryStream;

                await next(context);

                byte[] bytes = memoryStream.ToArray();

                if (memoryStream.Length > 0)
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    await memoryStream.CopyToAsync(originalStream);
                }

                options.Store.Set(key, new CachedResponse(response, bytes), options.Expiry);
            }
            finally
            {
                response.Body = originalStream;
            }
        }

        private async Task<bool> CheckCache(HttpContext context, CachingOptions options)
        {
            string key = options.Key.Get(context.Request);

            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (options.Store.TryGetValue(key, out CachedResponse cachedResponse))
            {
                await cachedResponse.MapToContext(context);
                return true;
            }

            return false;
        }
    }
}
