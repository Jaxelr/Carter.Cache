using System.Threading.Tasks;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sample.Carter.Cache.Application.Entities;

namespace Sample.Carter.Cache.Application.Modules;

public class MainModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/", (AppSettings settings, HttpContext ctx) =>
        {
            ctx.Response.Redirect(settings.RouteDefinition.RouteSuffix);
            return Task.CompletedTask;
        });
}
