namespace Sample.Carter.Cache.Redis.Application.Entities;

public record CacheConfig
{
    public bool CacheEnabled { get; set; }
    public int CacheTimespan { get; set; }
    public int CacheMaxSize { get; set; }
}
