using AspNetCoreRateLimit;

using JimCo.API.Infrastructure;
using JimCo.Common;
using JimCo.DataAccess.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureServices(builder.Configuration);

builder.Services.AddCors(options =>
  options.AddPolicy(name: "defaultCORS", builder =>
  builder.WithOrigins("http://localhost:3000", "https://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  app.Use(async (context, next) =>
  {
    Console.WriteLine($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "(null)"}");
    await next(context);
  });
}
else
{
  app.UseHsts();
}
app.UseCors("defaultCORS");

app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.ConfigureEndpoints();
if (settings.UpdateDatabase)
{
  await UpdateDatabase(app.Services.GetRequiredService<IDatabaseBuilder>());
}

app.Run();

static async Task UpdateDatabase(IDatabaseBuilder builder) => await builder.BuildDatabaseAsync(false);