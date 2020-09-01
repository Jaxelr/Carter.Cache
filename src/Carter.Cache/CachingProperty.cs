using System;

namespace Carter.Cache
{
    public class CachingProperty
    {
        public string CustomHeader { get; set; } = "X-Carter-Cache-Expiration";
        public TimeSpan Expiration { get; set; }
    }
}
