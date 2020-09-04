using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

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

                if (options.ValidResponse(ctx))
                {
                    AddEtag(ctx, bytes);
                }

                if (memoryStream.Length > 0)
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    await memoryStream.CopyToAsync(originalStream);
                }

                if (options.ValidResponse(ctx))
                {
                    await service.SetCache(ctx, new CachedResponse(ctx, bytes), options);
                }
            }
            finally
            {
                response.Body = originalStream;
            }
        }

        private void AddEtag(HttpContext ctx, byte[] bytes)
        {
            var defaultOption = CachingOption.DefaultValidResponse;
            if (defaultOption(ctx) && !ctx.Response.Headers.ContainsKey(HeaderNames.ETag))
            {
                using var md5 = MD5.Create();
                byte[] buffer = md5.ComputeHash(bytes);
                ctx.Response.Headers.Add(HeaderNames.ETag, $"\"{WebEncoders.Base64UrlEncode(buffer)}\"");
            }
        }
    }
}
