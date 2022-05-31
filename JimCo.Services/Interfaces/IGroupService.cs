
using JimCo.Common;
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IGroupService : IDataService<GroupModel>
{
  Task<IEnumerable<GroupModel>> GetAsync(string name);
  Task<IEnumerable<string>> GetGroupNamesAsync();
  Task<GroupModel?> ReadAsync(string name, string id);
  Task<GroupModel?> ReadForNameAsync(string name);
  Task<PopulatedGroupModel?> ReadGroupAsync(string name);
  Task<bool> UserHasGroupsAsync(string userid);
  Task<ApiError> RenameAsync(string name, string newname);
  Task<ApiError> AddUserToGroupAsync(string name, string userid);
  Task<ApiError> RemoveUserFromGroupAsync(string name, string userid);
}
