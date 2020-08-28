using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public class CachedResponse
    {
        public Dictionary<string, string> Headers { get; set; }
        public byte[] Body { get; set; }
        public long? ContentLength { get; set; }
        public int StatusCode { get; set; }
        public string ContentType { get; set; }
        public TimeSpan Expiry { get; set; }

        public CachedResponse(HttpResponse response, byte[] body)
        {
            Headers = new Dictionary<string, string>();
            Body = body;
            ContentType = response.ContentType;
            StatusCode = response.StatusCode;

            if (response.Headers.Keys.Contains("X-Carter-Cache-Expiration") && int.TryParse(response.Headers["X-Carter-Cache-Expiration"], out int value))
            {
                Expiry = TimeSpan.FromSeconds(value);

                Headers.Add("X-Carter-Cache-Expiration", response.Headers["X-Carter-Cache-Expiration"]);
            }
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
