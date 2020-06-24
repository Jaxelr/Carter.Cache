using System;
using Carter;
using Carter.Request;
using Carter.Response;
using Sample.Carter.Cache.Application.Entities;
using Sample.Carter.Cache.Application.Repository;

namespace Sample.Carter.Cache.Application.Modules
{
    public class HelloModule : CarterModule
    {
        public HelloModule(IHelloRepository repository)
        {
            Get<GetHello>("/Hello/{name}", async (ctx) =>
            {
                try
                {
                    string name = ctx.Request.RouteValues.As<string>("name");

                    string response = repository.SayHello(name);

                    if (response == null)
                    {
                        ctx.Response.StatusCode = 204;
                        return;
                    }

                    ctx.Response.StatusCode = 200;
                    await ctx.Response.Negotiate(response)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ctx.Response.StatusCode = 500;
                    await ctx.Response.Negotiate(new FailedResponse(ex))
                        .ConfigureAwait(false);
                }
            });
        }
    }
}
