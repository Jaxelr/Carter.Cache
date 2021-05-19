﻿using System.Threading.Tasks;
using Carter;
using Sample.Carter.Cache.Memcached.Application.Entities;

namespace Sample.Carter.Cache.Memcached.Application.Modules
{
    public class MainModule : CarterModule
    {
        public MainModule(AppSettings appSettings)
        {
            Get("/", (_, res) =>
            {
                res.Redirect(appSettings.RouteDefinition.RoutePrefix);

                return Task.CompletedTask;
            });
        }
    }
}
