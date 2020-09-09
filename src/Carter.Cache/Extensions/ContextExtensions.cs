using System;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public static class ContextExtensions
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
    }
}
