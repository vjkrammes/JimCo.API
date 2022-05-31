
using JimCo.Common;
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IUserService : IDataService<UserModel>
{
  Task<IEnumerable<UserModel>> GetForRoleAsync(string rolename);
  Task<IEnumerable<UserModel>> GetManagersAsync();
  Task<IEnumerable<UserModel>> GetAdminsAsync();
  Task<UserModel?> ReadForEmailAsync(string email);
  Task<UserModel?> ReadForIdentifierAsync(string identifier);
  Task<ApiError> AddRolesAsync(string email, params string[] roles);
  Task<ApiError> RemoveRolesAsync(string email, params string[] roles);
  Task<ApiError> ToggleRolesAsync(string email, params string[] roles);
  Task<string?> ReadNameForIdAsync(string id);
  Task<string?> ReadNameForIdentifierAsync(string identifier);
  Task<string?> ReadNameForEmailAsync(string email);
}
