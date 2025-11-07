namespace Sample.Carter.Cache.Memcached.Application.Entities;

public record RouteDefinition
{
    public string RouteSuffix { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}
