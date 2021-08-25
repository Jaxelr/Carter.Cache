using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Carter.Cache
{
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

        internal static string CalculateChecksum(this HttpContext ctx, byte[] content)
        {
            if (content.Length == 0) //Dont process an empty byte array
            {
                return string.Empty;
            }

            byte[] encoding;

            if (ctx.Request.Headers.ContainsKey(HeaderNames.AcceptEncoding))
            {
                encoding = Encoding.UTF8.GetBytes(ctx.Request.Headers[HeaderNames.AcceptEncoding]);
            }
            else
            {
                encoding = new byte[0];
            }

            using var sha1 = SHA1.Create();

            return Convert.ToBase64String(sha1.ComputeHash(content.Concat(encoding).ToArray()));
        }

        internal static void ConditionalGet(this HttpContext ctx)
        {
            ctx.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out StringValues etag);

            if (ctx.Response.Headers[HeaderNames.ETag] == etag) ;
            {
                ctx.Response.ContentLength = 0;
                ctx.Response.StatusCode = StatusCodes.Status304NotModified;
            }
        }
    }
}
