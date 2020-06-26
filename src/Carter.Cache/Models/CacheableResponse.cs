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

        public CachedResponse(HttpResponse response, byte[] body)
        {
            Headers = new Dictionary<string, string>();

            foreach (string key in response.Headers.Keys)
            {
                //TODO: Hmm...this is weird, must validate
                if (!Headers.ContainsKey(key) && key != "Transfer-Encoding")
                {
                    Headers[key] = response.Headers[key];
                }
            }

            Body = body;
            ContentType = response.ContentType;
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

            context.Response.ContentType = ContentType;
            context.Response.StatusCode = StatusCode;

            await context.Response.Body.WriteAsync(Body, 0, Body.Length);
        }
    }
}
