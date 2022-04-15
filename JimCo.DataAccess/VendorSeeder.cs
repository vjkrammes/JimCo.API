
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

using Microsoft.Extensions.Configuration;

namespace JimCo.DataAccess;
public class VendorSeeder : IVendorSeeder
{
  private readonly IVendorRepository _vendorRepository;

  public VendorSeeder(IVendorRepository vendorRepository) => _vendorRepository = vendorRepository;

  public async Task SeedAsync(IConfiguration configuration, string sectionName)
  {
    if (configuration is null || string.IsNullOrWhiteSpace(sectionName))
    {
      return;
    }
    var models = configuration.GetSection(sectionName).Get<VendorSeedModel[]>();
    if (models is null || !models.Any())
    {
      return;
    }
    foreach (var model in models)
    {
      if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Contact))
      {
        continue;
      }
      if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Phone) && string.IsNullOrWhiteSpace(model.Fax))
      {
        Console.WriteLine($"Can't seed vendor {model.Name}: At least one of Email, Phone or Fax is required");
        continue;
      }
      if (await _vendorRepository.ReadAsync(model.Name) is not null)
      {
        continue;
      }
      var vendor = new VendorEntity()
      {
        Id = 0,
        Name = model.Name,
        Address1 = model.Address1 ?? string.Empty,
        Address2 = model.Address2 ?? string.Empty,
        City = model.City ?? string.Empty,
        State = model.State ?? string.Empty,
        PostalCode = model.PostalCode ?? string.Empty,
        Contact = model.Contact,
        Email = model.Email ?? string.Empty,
        Phone = model.Phone ?? string.Empty,
        Fax = model.Fax ?? string.Empty
      };
      var result = await _vendorRepository.InsertAsync(vendor);
      if (!result.Successful)
      {
        Console.WriteLine($"Unable to seed vendor '{model.Name}': {result.ErrorMessage}");
      }
    }
  }
}
