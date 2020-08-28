using System;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public static class ContextExtensions
    {
        public static void Cacheable(this HttpContext context, int seconds)
        {
            var span = TimeSpan.FromSeconds(seconds);

            context.AddResponseExpirationHeader(span);
        }

        public static void Cacheable(this HttpContext context, TimeSpan span)
        {
            context.AddResponseExpirationHeader(span);
        }

        public static void Cacheable(this HttpContext context, DateTime absoluteExpiration)
        {
            var span = absoluteExpiration - DateTime.UtcNow;

            context.AddResponseExpirationHeader(span);
        }

        private static void AddResponseExpirationHeader(this HttpContext context, TimeSpan span)
        {
            context.Response.Headers.Add("X-Carter-Cache-Expiration", span.Seconds.ToString());
        }
    }
}
