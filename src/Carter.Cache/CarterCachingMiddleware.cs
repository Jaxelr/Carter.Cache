using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public class CarterCachingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly CachingOptions options;

        private readonly ICarterCachingService service;

        public CarterCachingMiddleware(RequestDelegate next, ICarterCachingService service, CachingOptions options) =>
            (this.next, this.service, this.options) = (next, service, options);

        public async Task Invoke(HttpContext context)
        {
            bool cacheHit = await service.CheckCache(context, options);

            if (!cacheHit)
            {
                await SetCache(context, next, options);
            }
        }

        private async Task SetCache(HttpContext context, RequestDelegate next, CachingOptions options)
        {
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

                await service.SetCache(context, new CachedResponse(response, bytes), options);
            }
            finally
            {
                response.Body = originalStream;
            }
        }
    }
}
