using System;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache;

public static class HttpRequestExtensions
{
    public static void AsCacheable(this HttpRequest req, int seconds, string customHeader = null)
    {
        var span = TimeSpan.FromSeconds(seconds);

        req.HttpContext.AddResponseExpirationHeader(span, customHeader);
    }

    public static void AsCacheable(this HttpRequest req, TimeSpan span, string customHeader = null) => req.HttpContext.AddResponseExpirationHeader(span, customHeader);

    public static void AsCacheable(this HttpRequest req, DateTime absoluteExpiration, string customHeader = null)
    {
        var span = absoluteExpiration - DateTime.UtcNow;

        req.HttpContext.AddResponseExpirationHeader(span, customHeader);
    }
}
