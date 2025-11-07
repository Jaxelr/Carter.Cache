namespace Sample.Carter.Cache.Application.Entities;

public record RouteDefinition
{
    public string RouteSuffix { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}
