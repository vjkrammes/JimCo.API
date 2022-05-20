
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IGroupService : IDataService<GroupModel>
{
  Task<IEnumerable<GroupModel>> GetAsync(string name);
  Task<IEnumerable<string>> GetGroupNamesAsync();
  Task<GroupModel?> ReadAsync(string name, string identifier);
  Task<PopulatedGroupModel?> ReadGroupAsync(string name);
}
