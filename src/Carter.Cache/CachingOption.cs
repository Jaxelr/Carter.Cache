using System;
using Carter.Cache.Keys;
using Carter.Cache.Stores;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public class CachingOption
    {
        public Func<HttpContext, bool> ValidRequest { get; set; } = DefaultValidRequest;
        public Func<HttpContext, bool> ValidResponse { get; set; } = DefaultValidResponse;

        public ICacheKey Key { get; set; }
        public ICacheStore Store { get; set; }

        public CachingOption() : this(new DefaultKeyGenerator(), new DefaultMemoryStore())
        {
        }

        public CachingOption(long maxSize) : this(new DefaultKeyGenerator(), new DefaultMemoryStore(maxSize))
        {
        }

        public CachingOption(ICacheKey key) : this(key, new DefaultMemoryStore())
        {
        }

        public CachingOption(ICacheStore store) : this(new DefaultKeyGenerator(), store)
        {
        }

        public CachingOption(ICacheKey key, ICacheStore store) => (Key, Store) = (key, store);

        internal static Func<HttpContext, bool> DefaultValidRequest = (ctx) => ctx.Request.Method == HttpMethods.Get;

        internal static Func<HttpContext, bool> DefaultValidResponse = (ctx) => ctx.Response.StatusCode == StatusCodes.Status200OK;
    }
}
