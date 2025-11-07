namespace Sample.Carter.Cache.Redis.Application.Entities;

/// <summary>
/// This is obtained from the appsettings.json on Startup
/// </summary>
public record AppSettings
{
    public RouteDefinition RouteDefinition { get; set; } = new();
    public string[] ServerUrls { get; set; } = [];
}
