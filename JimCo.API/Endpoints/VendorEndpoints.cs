using JimCo.Common;
using JimCo.Services.Interfaces;

namespace JimCo.API.Endpoints;

public static class VendorEndpoints
{
  public static void ConfigureVendorEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Vendor", GetVendors);
    app.MapGet("/api/v1/Vendor/ById/{vendorid}", ById);
    app.MapGet("/api/v1/Vendor/ByName/{name}", ByName);
  }

  private static async Task<IResult> GetVendors(IVendorService vendorService) =>
    Results.Ok((await vendorService.GetAsync()).OrderBy(x => x.Name));

  private static async Task<IResult> ById(string vendorid, IVendorService vendorService)
  {
    var vendor = await vendorService.ReadAsync(vendorid);
    return vendor is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "vendor", "id", vendorid)))
      : Results.Ok(vendor);
  }

  private static async Task<IResult> ByName(string name, IVendorService vendorService)
  {
    var vendor = await vendorService.ReadForNameAsync(name);
    return vendor is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "vendor", "name", name)))
      : Results.Ok(vendor);
  }
}
