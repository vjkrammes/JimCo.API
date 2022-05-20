
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IVendorRepository : IRepository<VendorEntity>
{
  Task<IEnumerable<VendorEntity>> PageVendorsAsync(int pageno, int pagesize, string columnName = "Id");
  Task<VendorEntity?> ReadAsync(string name);
  Task<IEnumerable<VendorEntity>> SearchAsync(string searchText);
  Task<IEnumerable<VendorEntity>> SearchContactAsync(string contact);
  Task<IEnumerable<VendorEntity>> SearchEmailAsync(string email);
}
