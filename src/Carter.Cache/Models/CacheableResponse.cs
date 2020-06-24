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

        public CachedResponse(HttpResponse response, byte[] body)
        {
            Headers = new Dictionary<string, string>();

            foreach (string key in response.Headers.Keys)
            {
                if (!Headers.ContainsKey(key))
                {
                    Headers[key] = response.Headers[key];
                }
            }

            Body = body;
            ContentLength = body.Length;
            StatusCode = response.StatusCode;
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

            context.Response.StatusCode = StatusCode;
            context.Response.ContentLength = ContentLength;
            await context.Response.Body.WriteAsync(Body, 0, Body.Length)
                .ConfigureAwait(false);
        }
    }
}
