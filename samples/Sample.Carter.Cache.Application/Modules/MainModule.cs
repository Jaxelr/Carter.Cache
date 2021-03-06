﻿using System.Threading.Tasks;
using Carter;
using Sample.Carter.Cache.Application.Entities;

namespace Sample.Carter.Cache.Application.Modules
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
