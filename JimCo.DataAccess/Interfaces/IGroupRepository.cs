
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IGroupRepository : IRepository<GroupEntity>
{
  Task<IEnumerable<GroupEntity>> GetAsync(string name);
  Task<IEnumerable<string>> GetGroupNamesAsync();
  Task<GroupEntity?> ReadAsync(string name, string identity);
}
