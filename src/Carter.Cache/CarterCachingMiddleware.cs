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
            bool cacheHit = await CheckCache(context, options).ConfigureAwait(false);

            if (cacheHit)
            {
                await next(context).ConfigureAwait(false);
            }
            else
            {
                await SetCache(context, next, options).ConfigureAwait(false);
            }
        }

        private async Task SetCache(HttpContext context, RequestDelegate next, CachingOptions options)
        {
            string key = options.Key.Get(context.Request);

            HttpResponse response = context.Response;
            Stream originalStream = response.Body;

            try
            {
                using var ms = new MemoryStream();
                response.Body = ms;

                await next(context).ConfigureAwait(false);

                byte[] bytes = ms.ToArray();

                if (ms.Length > 0)
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    await ms.CopyToAsync(originalStream).ConfigureAwait(false);
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

            var cachedResponse = options.Store.Get(key);

            if (cachedResponse != null)
            {
                await cachedResponse.MapToContext(context).ConfigureAwait(false);
                return true;
            }

            return false;
        }
    }
}
