using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Carter.Cache;

public static class HttpContextExtensions
{
    public static void AsCacheable(this HttpContext ctx, int seconds, string customHeader = null)
    {
        var span = TimeSpan.FromSeconds(seconds);

        ctx.AddResponseExpirationHeader(span, customHeader);
    }

    public static void AsCacheable(this HttpContext ctx, TimeSpan span, string customHeader = null) => ctx.AddResponseExpirationHeader(span, customHeader);

    public static void AsCacheable(this HttpContext ctx, DateTime absoluteExpiration, string customHeader = null)
    {
        var span = absoluteExpiration - DateTime.UtcNow;

        ctx.AddResponseExpirationHeader(span, customHeader);
    }

    internal static void AddResponseExpirationHeader(this HttpContext ctx, TimeSpan span, string customHeader = null)
    {
        var property = ctx.Features.Get<CachingProperty>() ?? new CachingProperty();

        if (customHeader != null)
        {
            property.CustomHeader = customHeader;
        }

        property.Expiration = span;
        ctx.Features.Set(property);
    }

    internal static void AddEtagToContext(this HttpContext ctx, string checksum)
    {
        if (ctx.Response.Headers.ContainsKey(HeaderNames.ETag) || ctx.Response.StatusCode > 299)
            return;

        ctx.Response.Headers[HeaderNames.ETag] = $"\"{checksum}\"";
    }
}
