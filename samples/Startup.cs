using System.Collections.Generic;
using Carter;
using Carter.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Carter.Cache.Application.Entities;
using Sample.Carter.Cache.Application.Repository;

namespace Sample.Carter.Cache.Application
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        private readonly AppSettings settings;

        private const string ServiceName = "Sample";

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

            Configuration = builder.Build();

            //Extract the AppSettings information from the appsettings config.
            settings = new AppSettings();
            Configuration.GetSection(nameof(AppSettings)).Bind(settings);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(settings); //typeof(AppSettings)

            services.AddSingleton<IHelloRepository>(new HelloRepository());

            services.AddLogging(opt =>
            {
                opt.ClearProviders();
                opt.AddConsole();
                opt.AddDebug();
                opt.AddConfiguration(Configuration.GetSection("Logging"));
            });

            services.AddCarterCaching();

            services.AddCarter(options => options.OpenApi = GetOpenApiOptions(settings));
        }

        public void Configure(IApplicationBuilder app, AppSettings appSettings)
        {
            app.UseRouting();

            app.UseSwaggerUI(opt =>
            {
                opt.RoutePrefix = appSettings.RouteDefinition.RoutePrefix;
                opt.SwaggerEndpoint(appSettings.RouteDefinition.SwaggerEndpoint, ServiceName);
            });

            app.UseCarterCaching();

            app.UseEndpoints(builder => builder.MapCarter());
        }

        private OpenApiOptions GetOpenApiOptions(AppSettings settings) =>
        new OpenApiOptions()
        {
            DocumentTitle = ServiceName,
            ServerUrls = settings.ServerUrls,
            Securities = new Dictionary<string, OpenApiSecurity>()
        };
    }
}
