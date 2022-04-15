
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IUserService : IDataService<UserModel>
{
  Task<UserModel?> ReadForEmailAsync(string email);
  Task<UserModel?> ReadForIdentifierAsync(string identifier);
}
