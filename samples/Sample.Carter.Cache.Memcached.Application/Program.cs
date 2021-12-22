using Carter;
using Carter.Cache;
using Carter.Cache.Memcached;
using Carter.Cache.Stores;
using Carter.OpenApi;
using Enyim.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Sample.Carter.Cache.Memcached.Application.Entities;
using Sample.Carter.Cache.Memcached.Application.Repository;

const string ServiceName = "Sample";
const string Policy = "DefaultPolicy";
const string LOCALHOST = "127.0.0.1";
const int PORT = 11211;

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

builder.Services.AddCarter();

//Dependencies
builder.Services.AddEnyimMemcached(options => options.AddServer(LOCALHOST, PORT));
builder.Services.AddSingleton<ICacheStore>(provider => new MemcachedStore(provider.GetRequiredService<IMemcachedClient>()));
builder.Services.AddSingleton(provider => new CachingOption() { Store = provider.GetRequiredService<ICacheStore>() });

builder.Services.AddSingleton<IHelloRepository, HelloRepository>();

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

app.UseEndpoints(builder => builder.MapCarter());

app.Run();
