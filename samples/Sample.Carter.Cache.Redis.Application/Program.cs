using System;
using Carter;
using Carter.Cache;
using Carter.Cache.Redis;
using Carter.Cache.Stores;
using Carter.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Sample.Carter.Cache.Redis.Application.Entities;
using Sample.Carter.Cache.Redis.Application.Repository;

const string ServiceName = "Sample";
const string Policy = "DefaultPolicy";
const string REDIS_LOCALHOST = "127.0.0.1:6379";

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

//Dependencies
builder.Services.AddSingleton(settings); //AppSettings
builder.Services.AddSingleton<ICacheStore>(new RedisStore(REDIS_LOCALHOST));
builder.Services.AddSingleton(provider => new CachingOption() { Store = provider.GetRequiredService<ICacheStore>() });

builder.Services.AddSingleton<IHelloRepository, HelloRepository>();

IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

builder.Services.AddCarterCaching(serviceProvider.GetRequiredService<CachingOption>());
builder.Services.AddCarter();

//HealthChecks
builder.Services.AddHealthChecks();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(settings.RouteDefinition.Version, new OpenApiInfo
    {
        Description = ServiceName,
        Version = settings.RouteDefinition.Version,
    });

    options.DocInclusionPredicate((_, description) =>
    {
        foreach (object metaData in description.ActionDescriptor.EndpointMetadata)
        {
            if (metaData is IIncludeOpenApi)
            {
                return true;
            }
        }
        return false;
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseCarterCaching();
app.MapCarter();

app.Run();
