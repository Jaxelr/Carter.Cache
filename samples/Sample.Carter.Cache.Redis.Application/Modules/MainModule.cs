using System.Threading.Tasks;
using Carter;
using Sample.Carter.Cache.Redis.Application.Entities;

namespace Sample.Carter.Cache.Redis.Application.Modules
{
    public class MainModule : CarterModule
    {
        public MainModule(AppSettings appSettings)
        {
            Get("/", (ctx) =>
            {
                ctx.Response.Redirect(appSettings.RouteDefinition.RoutePrefix);

                return Task.CompletedTask;
            });
        }
    }
}
