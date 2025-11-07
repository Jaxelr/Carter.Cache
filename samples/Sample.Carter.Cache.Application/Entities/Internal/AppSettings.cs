namespace Sample.Carter.Cache.Application.Entities;

/// <summary>
/// This is obtained from the appsettings.json on Startup
/// </summary>
public record AppSettings
{
    public CacheConfig Cache { get; set; } = new();
    public RouteDefinition RouteDefinition { get; set; } = new();
    public string[] ServerUrls { get; set; } = [];
}
