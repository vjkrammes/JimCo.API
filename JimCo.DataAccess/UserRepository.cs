using System.Data;

using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess;
public class UserRepository : RepositoryBase<UserEntity>, IUserRepository
{
  public UserRepository(IDatabase database) : base(database) { }

  public async Task<UserEntity?> ReadAsync(string email)
  {
    var sql = "select * from Users where Email=@email;";
    return await ReadAsync(sql, new QueryParameter("email", email, DbType.String));
  }

  public async Task<UserEntity?> ReadForIdentifierAsync(string identifier)
  {
    var sql = "select * from Users where Identifier=@identifier;";
    return await ReadAsync(sql, new QueryParameter("identifier", identifier, DbType.String));
  }
}
