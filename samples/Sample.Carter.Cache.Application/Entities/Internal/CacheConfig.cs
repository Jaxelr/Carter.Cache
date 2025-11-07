namespace Sample.Carter.Cache.Application.Entities;

public record CacheConfig
{
    public bool CacheEnabled { get; set; } = true;
    public int CacheTimespan { get; set; } = 60;
    public int CacheMaxSize { get; set; } = 2048;
}
