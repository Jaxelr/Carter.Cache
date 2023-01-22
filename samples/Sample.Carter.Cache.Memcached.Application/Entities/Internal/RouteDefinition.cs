namespace Sample.Carter.Cache.Memcached.Application.Entities;

public record RouteDefinition
{
    public string RouteSuffix { get; set; }
    public string Version { get; set; }
}
