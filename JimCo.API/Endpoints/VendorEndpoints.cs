using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JimCo.API.Endpoints;

public static class VendorEndpoints
{
  public static void ConfigureVendorEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Vendor", GetVendors);
    app.MapGet("/api/v1/Vendor/ById/{vendorid}", ById);
    app.MapGet("/api/v1/Vendor/ByName/{name}", ByName);
    app.MapGet("/api/v1/Vendor/ByEmail/{email}", ByEmail);
    app.MapGet("/api/v1/Vendor/Page/{pageno}/{pagesize}", PageVendors);
    app.MapPost("/api/v1/Vendor/Create/{addRole}", CreateVendor).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Vendor/Update/{toggleRole}", UpdateVendor).RequireAuthorization("ManagerPlusRequired");
    app.MapDelete("/api/v1/Vendor/Delete/{id}", DeleteVendor).RequireAuthorization("ManagerPlusRequired");
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
  
  private static async Task<IResult> ByEmail(string email, IVendorService vendorService)
  {
    var vendor = await vendorService.ReadForEmailAsync(email);
    return vendor is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "vendor", "email", email)))
      : Results.Ok(vendor);
  }

  private static async Task<IResult> PageVendors(int pageno, int pagesize, IVendorService vendorService)
  {
    var ret = await vendorService.PageVendorsAsync(pageno, pagesize, "Name");
    return Results.Ok(ret);
  }

  private static async Task<IResult> CreateVendor([FromBody]VendorModel model, bool addRole, IVendorService vendorService, IUserService userService,
    IUriHelper helper, IOptions<AppSettings> settings)
  {
    if (model is null)
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    helper.SetBase(settings.Value.ApiBase);
    helper.SetVersion(1);
    var result = await vendorService.InsertAsync(model);
    if (result.Successful)
    {
      if (addRole)
      {
        await userService.AddRolesAsync(model.Email, "Vendor");
      }
      var uri = helper.Create("Vendor", "ById", model.Id);
      return Results.Created(uri.ToString(), model);
    }
    return Results.BadRequest(result);
  }

  private static async Task<IResult> UpdateVendor([FromBody]VendorModel model, bool toggleRole, IVendorService vendorService, IUserService userService)
  {
    if (model is null)
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var result = await vendorService.UpdateAsync(model);
    if (result.Successful)
    {
      if (toggleRole)
      {
        await userService.ToggleRolesAsync(model.Email, "Vendor");
      }
      return Results.Ok(model);
    }
    return Results.BadRequest(result);
  }

  private static async Task<IResult> DeleteVendor(string id, IVendorService vendorService)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "vendor", "id", id)));
    }
    var vendor = await vendorService.ReadAsync(id);
    if (vendor is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "vendor", "id", id)));
    }
    var result = await vendorService.DeleteAsync(vendor);
    if (result.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(result);
  }
}
