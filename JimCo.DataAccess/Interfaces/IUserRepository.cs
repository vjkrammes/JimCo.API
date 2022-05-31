
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IUserRepository : IRepository<UserEntity>
{
  Task<IEnumerable<UserEntity>> GetForRoleAsync(string rolename);
  Task<UserEntity?> ReadAsync(string email);
  Task<UserEntity?> ReadForIdentifierAsync(string identifier);
  Task<string?> ReadNameForIdAsync(int id);
  Task<string?> ReadNameForIdentifierAsync(string identifier);
  Task<string?> ReadNameForEmailAsync(string email);
  Task<DalResult> AddRolesAsync(string email, params string[] roles);
  Task<DalResult> AddRolesAsync(int id, params string[] roles);
  Task<DalResult> RemoveRolesAsync(string email, params string[] roles);
  Task<DalResult> RemoveRolesAsync(int id, params string[] roles);
  Task<DalResult> ToggleRolesAsync(string email, params string[] roles);
  Task<DalResult> ToggleRolesAsync(int id, params string[] roles);
}
