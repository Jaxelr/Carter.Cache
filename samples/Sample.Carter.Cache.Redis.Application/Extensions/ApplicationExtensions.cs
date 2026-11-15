using System;
using System.Threading.Tasks;
using Carter;
using Carter.Cache;
using Carter.Cache.Redis;
using Carter.Cache.Stores;
using Carter.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Sample.Carter.Cache.Redis.Application.Entities;
using Sample.Carter.Cache.Redis.Application.Repository;
using Scalar.AspNetCore;

namespace Sample.Carter.Cache.Redis.Application.Extensions;

internal static class ApplicationExtensions
{
    private const string ServiceName = "Sample";
    private const string Policy = "DefaultPolicy";
    private const string RedisLocalhost = "127.0.0.1:6379";

    internal static AppSettings AddSampleServices(this WebApplicationBuilder builder)
    {
        var settings = new AppSettings();
        builder.Configuration.GetSection(nameof(AppSettings)).Bind(settings);

        ConfigureCors(builder.Services);
        ConfigureLogging(builder);
        AddApplicationServices(builder.Services, settings);

        //HealthChecks
        builder.Services.AddHealthChecks();
        AddOpenApi(builder.Services, settings);

        return settings;
    }

    internal static WebApplication UseSample(this WebApplication app, AppSettings settings, IWebHostEnvironment environment)
    {
        app.UseCors(Policy);

        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.MapOpenApi();
        app.MapScalarApiReference(settings.RouteDefinition.RouteSuffix, options => options
            .WithTitle(ServiceName)
            .AddDocument(settings.RouteDefinition.Version, ServiceName));
        app.UseCarterCaching();
        app.MapCarter();

        return app;
    }

    private static void ConfigureCors(IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy(Policy, builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

    private static void ConfigureLogging(WebApplicationBuilder builder) =>
        builder.Services.AddLogging(options =>
        {
            options.ClearProviders();
            options.AddConsole();
            options.AddDebug();
            options.AddConfiguration(builder.Configuration.GetSection("Logging"));
        });

    private static void AddApplicationServices(IServiceCollection services, AppSettings settings)
    {
        //Dependencies
        services.AddSingleton(settings); //AppSettings
        services.AddSingleton<ICacheStore>(new RedisStore(RedisLocalhost));
        services.AddSingleton(provider => new CachingOption()
        {
            Store = provider.GetRequiredService<ICacheStore>(),
        });
        services.AddSingleton<IHelloRepository, HelloRepository>();

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        services.AddCarterCaching(serviceProvider.GetRequiredService<CachingOption>());
        services.AddCarter();
    }

    private static void AddOpenApi(IServiceCollection services, AppSettings settings) =>
        services.AddOpenApi(settings.RouteDefinition.Version, options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
            options.ShouldInclude = ShouldInclude;
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Description = ServiceName,
                    Title = ServiceName,
                    Version = settings.RouteDefinition.Version,
                };

                return Task.CompletedTask;
            });
        });

    private static bool ShouldInclude(ApiDescription description)
    {
        foreach (object metaData in description.ActionDescriptor.EndpointMetadata)
        {
            if (metaData is IIncludeOpenApi)
            {
                return true;
            }
        }

        return false;
    }
}
