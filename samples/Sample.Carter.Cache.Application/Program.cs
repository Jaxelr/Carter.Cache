using System.Threading.Tasks;
using Carter;
using Carter.Cache;
using Carter.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Sample.Carter.Cache.Application.Entities;
using Sample.Carter.Cache.Application.Repository;
using Scalar.AspNetCore;

const string ServiceName = "Sample";
const string Policy = "DefaultPolicy";

var builder = WebApplication.CreateBuilder(args);

var settings = new AppSettings();

builder.Configuration.GetSection(nameof(AppSettings)).Bind(settings);

builder.Services.AddCors(options =>
{
    options.AddPolicy(Policy,
    builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddLogging(opt =>
{
    opt.ClearProviders();
    opt.AddConsole();
    opt.AddDebug();
    opt.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

builder.Services.AddCarterCaching(new CachingOption(2048));
builder.Services.AddCarter();

//Dependencies
builder.Services.AddSingleton(settings); //AppSettings
builder.Services.AddSingleton<IHelloRepository, HelloRepository>();

//HealthChecks
builder.Services.AddHealthChecks();

builder.Services.AddOpenApi(settings.RouteDefinition.Version, options =>
{
    options.ShouldInclude = description =>
    {
        foreach (object metaData in description.ActionDescriptor.EndpointMetadata)
        {
            if (metaData is IIncludeOpenApi)
            {
                return true;
            }
        }
        return false;
    };

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

var app = builder.Build();

app.UseCors(Policy);

if (builder.Environment.IsDevelopment())
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

await app.RunAsync();
