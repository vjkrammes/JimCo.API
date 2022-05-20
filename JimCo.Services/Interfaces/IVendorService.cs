using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IVendorService : IDataService<VendorModel>
{
  Task<IEnumerable<VendorModel>> PageVendorsAsync(int pageno, int pagesize, string columnName = "Id");
  Task<IEnumerable<VendorModel>> SearchAsync(string searchText);
  Task<IEnumerable<VendorModel>> SearchForContactAsync(string contact);
  Task<IEnumerable<VendorModel>> SearchForEmailAsync(string email);
  Task<VendorModel?> ReadForNameAsync(string name);
}
