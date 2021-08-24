using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public class CarterCachingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly CachingOption options;
        private readonly ICarterCachingService service;

        public CarterCachingMiddleware(RequestDelegate next, ICarterCachingService service, CachingOption options) =>
            (this.next, this.service, this.options) = (next, service, options);

        public async Task Invoke(HttpContext ctx)
        {
            if (options.ValidRequest(ctx))
            {
                bool cacheHit = await service.CheckCache(ctx, options);

                if (!cacheHit)
                {
                    await SetCache(ctx, next, options);
                }
            }
            else
            {
                //If its an invalid request based on our logic, carry on.
                await next(ctx);
            }
        }

        private async Task SetCache(HttpContext ctx, RequestDelegate next, CachingOption options)
        {
            var response = ctx.Response;
            var originalStream = response.Body;

            try
            {
                using var memoryStream = new MemoryStream();
                response.Body = memoryStream;

                await next(ctx);

                byte[] bytes = memoryStream.ToArray();

                if (memoryStream.Length > 0)
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    await memoryStream.CopyToAsync(originalStream);
                }

                if (options.ValidResponse(ctx))
                {
                    string checksum = ctx.CalculateChecksum(bytes);
                    ctx.AddEtagToContext(checksum);
                    await service.SetCache(ctx, new CachedResponse(ctx, bytes), options);
                }
            }
            finally
            {
                response.Body = originalStream;
            }
        }
    }
}
