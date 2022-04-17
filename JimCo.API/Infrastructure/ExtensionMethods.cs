
using AspNetCoreRateLimit;

using JimCo.API.Endpoints;
using JimCo.API.Models;
using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.DataAccess;
using JimCo.DataAccess.Interfaces;
using JimCo.Services;
using JimCo.Services.Interfaces;

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
    services.AddTransient<ILineItemRepository, LineItemRepository>();
    services.AddTransient<IOrderRepository, OrderRepository>();
    services.AddTransient<IProductRepository, ProductRepository>();
    services.AddTransient<IPromotionRepository, PromotionRepository>();
    services.AddTransient<ISystemSettingsRepository, SystemSettingsRepository>();
    services.AddTransient<IUserRepository, UserRepository>();
    services.AddTransient<IVendorRepository, VendorRepository>();

    // seeders

    services.AddTransient<IAlertSeeder, AlertSeeder>();
    services.AddTransient<ICategorySeeder, CategorySeeder>();
    services.AddTransient<ILineItemSeeder, LineItemSeeder>();
    services.AddTransient<IOrderSeeder, OrderSeeder>();
    services.AddTransient<IProductSeeder, ProductSeeder>();
    services.AddTransient<IPromotionSeeder, PromotionSeeder>();
    services.AddTransient<ISystemSettingsSeeder, SystemSettingsSeeder>();
    services.AddTransient<IUserSeeder, UserSeeder>();
    services.AddTransient<IVendorSeeder, VendorSeeder>();

    // data services

    services.AddTransient<IAlertService, AlertService>();
    services.AddTransient<ICategoryService, CategoryService>();
    services.AddTransient<ILineItemService, LineItemService>();
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
    app.ConfigureLineItemEndpoints();
    app.ConfigureOrderEndpoints();
    app.ConfigureProductEndpoints();
    app.ConfigurePromotionEndpoints();
    app.ConfigureUserEndpoints();
    app.ConfigureSystemSettingsEndpoints();
    app.ConfigureVendorEndpoints();
  }
}
