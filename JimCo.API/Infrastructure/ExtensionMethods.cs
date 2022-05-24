
using System.IdentityModel.Tokens.Jwt;

using AspNetCoreRateLimit;

using JimCo.API.Authorization;
using JimCo.API.Endpoints;
using JimCo.API.Models;
using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.DataAccess;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Newtonsoft.Json;

namespace JimCo.API.Infrastructure;

public static class ExtensionMethods
{
  public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
  {
    // authentication and authorization

    // rate limiting

    services.AddOptions();
    services.AddMemoryCache();
    services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimit"));
    services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
    services.AddInMemoryRateLimiting();
    services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

    // IDatabase and IDatabaseBuilder

    var dbsettings = configuration.GetSection("Database").Get<DatabaseSettings>();
    if (dbsettings is null)
    {
      dbsettings = new();
    }
    services.AddTransient<IDatabase>(x => new Database(dbsettings.Server, dbsettings.Name, dbsettings.Auth));
    services.AddTransient<IDatabaseBuilder, DatabaseBuilder>();

    // app settings

    services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
    var settings = configuration.GetSection("AppSettings").Get<AppSettings>();
    if (settings is null)
    {
      settings = new();
    }

    // authentication and authorization

    services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
      .AddJwtBearer(options =>
      {
        options.Authority = configuration["Auth0:Authority"];
        options.Audience = configuration["Auth0:Audience"];
      });

    services.AddAuthorization(options =>
    {
      options.AddPolicy("AdminRequired", 
        policy => policy.Requirements.Add(new MustHaveRoleRequirement("Admin")));
      options.AddPolicy("ManagerRequired", 
        policy => policy.Requirements.Add(new MustHaveRoleRequirement("Manager")));
      options.AddPolicy("EmployeeRequired", 
        policy => policy.Requirements.Add(new MustHaveRoleRequirement("Employee")));
      options.AddPolicy("ManagerPlusRequired", 
        policy => policy.Requirements.Add(new OneRoleRequirement("Manager", "Admin")));
      options.AddPolicy("JimCoEmployee",
        policy => policy.Requirements.Add(new OneRoleRequirement("Employee", "Manager", "Admin")));
    });

    services.AddScoped<IAuthorizationHandler, MustHaveRoleHandler>();
    services.AddScoped<IAuthorizationHandler, OneRoleHandler>();
    services.AddScoped<IAuthorizationHandler, AllRolesHandler>();

    // miscellaneous services

    services.AddSingleton<IColorService, ColorService>();
    services.AddTransient<IConfigurationFactory, ConfigurationFactory>();
    services.AddHttpContextAccessor();
    services.AddTransient<IHttpStatusCodeTranslator, HttpStatusCodeTranslator>();
    services.AddTransient<ILoremIpsumGenerator, LoremIpsumGenerator>();
    services.AddTransient<IPasswordChecker, PasswordChecker>();
    services.AddTransient<ISkuGenerator, SkuGenerator>();
    services.AddTransient<ITimeSpanConverter, TimeSpanConverter>();
    services.AddTransient<IUriHelper, UriHelper>();

    // data repositories

    services.AddTransient<IAlertRepository, AlertRepository>();
    services.AddTransient<ICategoryRepository, CategoryRepository>();
    services.AddTransient<IGroupRepository, GroupRepository>();
    services.AddTransient<ILineItemRepository, LineItemRepository>();
    services.AddTransient<ILogRepository, LogRepository>();
    services.AddTransient<IOrderRepository, OrderRepository>();
    services.AddTransient<IProductRepository, ProductRepository>();
    services.AddTransient<IPromotionRepository, PromotionRepository>();
    services.AddTransient<ISystemSettingsRepository, SystemSettingsRepository>();
    services.AddTransient<IUserRepository, UserRepository>();
    services.AddTransient<IVendorRepository, VendorRepository>();

    // seeders

    services.AddTransient<IAlertSeeder, AlertSeeder>();
    services.AddTransient<ICategorySeeder, CategorySeeder>();
    services.AddTransient<IGroupSeeder, GroupSeeder>();
    services.AddTransient<ILineItemSeeder, LineItemSeeder>();
    services.AddTransient<ILogSeeder, LogSeeder>();
    services.AddTransient<IOrderSeeder, OrderSeeder>();
    services.AddTransient<IProductSeeder, ProductSeeder>();
    services.AddTransient<IPromotionSeeder, PromotionSeeder>();
    services.AddTransient<ISystemSettingsSeeder, SystemSettingsSeeder>();
    services.AddTransient<IUserSeeder, UserSeeder>();
    services.AddTransient<IVendorSeeder, VendorSeeder>();

    // data services

    services.AddTransient<IAlertService, AlertService>();
    services.AddTransient<ICategoryService, CategoryService>();
    services.AddTransient<IGroupService, GroupService>();
    services.AddTransient<ILineItemService, LineItemService>();
    services.AddTransient<ILogService, LogService>();
    services.AddTransient<IOrderService, OrderService>();
    services.AddTransient<IProductService, ProductService>();
    services.AddTransient<IPromotionService, PromotionService>();
    services.AddTransient<ISystemSettingsService, SystemSettingsService>();
    services.AddTransient<IUserService, UserService>();
    services.AddTransient<IVendorService, VendorService>();

    return services;
  }

  public static void ConfigureEndpoints(this WebApplication app)
  {
    app.ConfigureAlertEndpoints();
    app.ConfigureCategoryEndpoints();
    app.ConfigureGroupEndpoints();
    app.ConfigureLineItemEndpoints();
    app.ConfigureLoggingEndpoints();
    app.ConfigureOrderEndpoints();
    app.ConfigureProductEndpoints();
    app.ConfigurePromotionEndpoints();
    app.ConfigureUserEndpoints();
    app.ConfigureSystemSettingsEndpoints();
    app.ConfigureVendorEndpoints();
  }

  public static bool HasRole(this UserModel user, string rolename)
  {
    if (user is null || string.IsNullOrWhiteSpace(user.JobTitles))
    {
      return false;
    }
    try
    {
      var roles = JsonConvert.DeserializeObject<string[]>(user.JobTitles);
      if (roles is null || !roles.Any())
      {
        return false;
      }
      return roles.Contains(rolename, StringComparer.OrdinalIgnoreCase);
    }
    catch
    {
      return false;
    }
  }

  public static bool IsManager(this UserModel user) => user.HasRole("manager");

  public static bool IsAdmin(this UserModel user) => user.HasRole("admin");

  public static string? GetTokenString(this HttpRequest request) => request?.Headers["Authorization"].FirstOrDefault()?.Split(new char[] {' '}).Last();

  public static string? GetTokenString(this HttpContext context) => context?.Request.GetTokenString();

  public static JwtSecurityToken? GetToken(this HttpRequest request)
  {
    var token = request.GetTokenString();
    if (string.IsNullOrWhiteSpace(token))
    {
      return null;
    }
    try
    {
      var handler = new JwtSecurityTokenHandler();
      return handler.ReadJwtToken(token);
    }
    catch
    {
      return null;
    }
  }

  public static JwtSecurityToken? GetToken(this HttpContext context) => context?.Request.GetToken();
}
