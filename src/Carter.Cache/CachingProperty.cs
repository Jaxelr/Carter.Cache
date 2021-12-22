using System;

namespace Carter.Cache;

public class CachingProperty
{
    private const string DefaultHeader = "X-Carter-Cache-Expiration";

    public string CustomHeader { get; set; } = DefaultHeader;
    public TimeSpan Expiration { get; set; }
}
