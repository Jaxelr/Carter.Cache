namespace Sample.Carter.Cache.Memcached.Application.Entities;

/// <summary>
/// This is obtained from the appsettings.json on Startup
/// </summary>
public record AppSettings
{
    public CacheConfig Cache { get; set; }
    public RouteDefinition RouteDefinition { get; set; }
    public string[] ServerUrls { get; set; }
}
