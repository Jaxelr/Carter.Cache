using System;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public static class ContextExtensions
    {
        public static void AsCacheable(this HttpContext context, int seconds, string customHeader = null)
        {
            var span = TimeSpan.FromSeconds(seconds);

            context.AddResponseExpirationHeader(span, customHeader);
        }

        public static void AsCacheable(this HttpContext context, TimeSpan span, string customHeader = null) => context.AddResponseExpirationHeader(span, customHeader);

        public static void AsCacheable(this HttpContext context, DateTime absoluteExpiration, string customHeader = null)
        {
            var span = absoluteExpiration - DateTime.UtcNow;

            context.AddResponseExpirationHeader(span, customHeader);
        }

        private static void AddResponseExpirationHeader(this HttpContext context, TimeSpan span, string customHeader = null)
        {
            var property = context.Features.Get<CachingProperty>() ?? new CachingProperty();

            if (customHeader != null)
            {
                property.CustomHeader = customHeader;
            }

            property.Expiration = span;
            context.Features.Set(property);
        }
    }
}
