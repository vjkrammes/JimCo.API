using AspNetCoreRateLimit;

using JimCo.API.Infrastructure;
using JimCo.Common;
using JimCo.DataAccess.Interfaces;

using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureServices(builder.Configuration);

var origins = builder.Configuration.GetSection("CORSOrigins").Get<string[]>();
if (origins is null || !origins.Any())
{
  builder.Services.AddCors(options =>
  options.AddDefaultPolicy(builder =>
    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
}
else
{
  origins.ForEach(x => Console.WriteLine(x));
  builder.Services.AddCors(options =>
    options.AddDefaultPolicy(builder =>
    builder.WithOrigins(origins)
      .AllowAnyHeader()
      .AllowAnyMethod()));
}

var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

var app = builder.Build();

app.UseIpRateLimiting();

if (app.Environment.IsProduction())
{
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
  app.Use(async (context, next) =>
  {
    Console.WriteLine($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "(null)"}");
    await next(context);
  });
}

app.ConfigureEndpoints();

if (settings.UpdateDatabase)
{
  await UpdateDatabase(app.Services.GetRequiredService<IDatabaseBuilder>());
}

app.Run();

static async Task UpdateDatabase(IDatabaseBuilder builder) => await builder.BuildDatabaseAsync(false);