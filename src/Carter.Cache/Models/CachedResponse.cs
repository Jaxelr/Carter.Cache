using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Carter.Cache;

public class CachedResponse
{
    public Dictionary<string, string> Headers { get; set; }
    public CachingProperty Property { get; set; }
    public byte[] Body { get; set; }
    public long? ContentLength { get; set; }
    public int StatusCode { get; set; }
    public string ContentType { get; set; }
    public TimeSpan Expiry { get; set; }

    public CachedResponse()
    {
        //For mapping from the cache store.
    }

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

    public async Task MapToContext(HttpContext ctx)
    {
        foreach (string headerKey in Headers.Keys)
        {
            if (!ctx.Response.Headers.ContainsKey(headerKey.ToLowerInvariant()))
            {
                ctx.Response.Headers[headerKey] = Headers[headerKey];
            }
        }

        ctx.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out StringValues etag);

        if (!string.IsNullOrWhiteSpace(etag) && ctx.Response.Headers[HeaderNames.ETag] == etag)
        {
            ctx.Response.ContentLength = 0;
            ctx.Response.StatusCode = StatusCodes.Status304NotModified;
        }
        else
        {
            ctx.Response.ContentType = ContentType;
            ctx.Response.StatusCode = StatusCode;
            await ctx.Response.Body.WriteAsync(Body, 0, Body.Length);
        }
    }
}
