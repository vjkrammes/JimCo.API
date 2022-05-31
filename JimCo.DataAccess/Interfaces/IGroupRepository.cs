
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IGroupRepository : IRepository<GroupEntity>
{
  Task<IEnumerable<GroupEntity>> GetAsync(string name);
  Task<IEnumerable<string>> GetGroupNamesAsync();
  Task<GroupEntity?> ReadAsync(string name, int userid);
  Task<GroupEntity?> ReadForNameAsync(string name);
  Task<bool> UserHasGroupsAsync(int userid);
  Task<DalResult> RenameAsync(string name, string newname);
  Task<DalResult> AddUserToGroupAsync(string groupname, int userid);
  Task<DalResult> RemoveUserFromGroupAsync(string groupname, int userid);
}
