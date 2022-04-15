
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IUserRepository : IRepository<UserEntity>
{
  Task<UserEntity?> ReadAsync(string email);
  Task<UserEntity?> ReadForIdentifierAsync(string identifier);
}
