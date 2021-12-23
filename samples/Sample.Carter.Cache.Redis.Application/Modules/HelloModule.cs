using System;
using Carter;
using Carter.Cache;
using Carter.OpenApi;
using Carter.Response;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sample.Carter.Cache.Redis.Application.Entities;
using Sample.Carter.Cache.Redis.Application.Repository;

namespace Sample.Carter.Cache.Redis.Application.Modules
{
    public class HelloModule : ICarterModule
    {
        private const string UserTag = "Hello";

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/Hello/{name}", async (string name, IHelloRepository repository, HttpContext ctx) =>
            {
                try
                {
                    ctx.AsCacheable(10);

                    string response = repository.SayHello(name);

                    if (response == null)
                    {
                        ctx.Response.StatusCode = 204;
                        return;
                    }

                    ctx.Response.StatusCode = 200;
                    await ctx.Response.Negotiate(response);
                }
                catch (Exception ex)
                {
                    ctx.Response.StatusCode = 500;
                    await ctx.Response.Negotiate(new FailedResponse(ex));
                }
            })
            .Produces<string>(200)
            .Produces(204)
            .Produces<FailedResponse>(500)
            .WithName("Hello")
            .WithTags(UserTag)
            .IncludeInOpenApi();

            app.MapGet("/Hello2/{name}", async (string name, IHelloRepository repository, HttpContext ctx) =>
            {
                try
                {
                    ctx.AsCacheable(15);

                    string response = repository.SayHello(name);

                    if (response == null)
                    {
                        ctx.Response.StatusCode = 204;
                        return;
                    }

                    ctx.Response.StatusCode = 200;
                    await ctx.Response.Negotiate(response);
                }
                catch (Exception ex)
                {
                    ctx.Response.StatusCode = 500;
                    await ctx.Response.Negotiate(new FailedResponse(ex));
                }
            })
            .Produces<string>(200)
            .Produces(204)
            .Produces<FailedResponse>(500)
            .WithName("Hello2")
            .WithTags(UserTag)
            .IncludeInOpenApi();
        }
    }
}
