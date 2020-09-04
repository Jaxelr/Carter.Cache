using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Carter.Cache
{
    public class CachedResponse
    {
        public Dictionary<string, string> Headers { get; set; }
        public CachingProperty Property { get; set; }
        public byte[] Body { get; set; }
        public long? ContentLength { get; set; }
        public int StatusCode { get; set; }
        public string ContentType { get; set; }
        public TimeSpan Expiry { get; set; }

        public CachedResponse(HttpContext ctx, byte[] body)
        {
            var property = ctx.Features.Get<CachingProperty>();

            if (property is null) //No caching properties assigned, return with no mapping made.
            {
                return;
            }

            var response = ctx.Response;

            Headers = new Dictionary<string, string>();
            Body = body;
            ContentType = response.ContentType;
            StatusCode = response.StatusCode;
            Expiry = property.Expiration;

            if (ctx.Response.Headers.ContainsKey(HeaderNames.ETag))
            {
                Headers.Add(HeaderNames.ETag, ctx.Response.Headers[HeaderNames.ETag]);
            }

            Headers.Add(property.CustomHeader, property.Expiration.ToString());
        }

        public async Task MapToContext(HttpContext context)
        {
            foreach (string headerKey in Headers.Keys)
            {
                if (!context.Response.Headers.ContainsKey(headerKey))
                {
                    context.Response.Headers[headerKey] = Headers[headerKey];
                }
            }

            context.Response.ContentType = ContentType;
            context.Response.StatusCode = StatusCode;

            await context.Response.Body.WriteAsync(Body, 0, Body.Length);
        }
    }
}
