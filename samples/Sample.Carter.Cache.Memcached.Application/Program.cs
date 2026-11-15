using Microsoft.AspNetCore.Builder;
using Sample.Carter.Cache.Memcached.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.AddSampleServices();
var app = builder.Build();

app.UseSample(settings, builder.Environment);
await app.RunAsync();
