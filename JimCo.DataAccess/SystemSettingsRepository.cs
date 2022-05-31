
using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;

namespace JimCo.DataAccess;
public class SystemSettingsRepository : RepositoryBase<SystemSettingsEntity>, ISystemSettingsRepository
{
  public SystemSettingsRepository(IDatabase database) : base(database) { }

  public override async Task<DalResult> InsertAsync(SystemSettingsEntity entity)
  {
    var existing = await GetAsync();
    if (existing is not null && existing.Any())
    {
      return DalResult.Duplicate;
    }
    return await base.InsertAsync(entity);
  }

  public async override Task<DalResult> UpdateAsync(SystemSettingsEntity entity)
  {
    if (entity is null)
    {
      return DalResult.FromException(new ArgumentNullException(nameof(entity)));
    }
    return await base.UpdateAsync(entity);
  }

  public override Task<DalResult> DeleteAsync(SystemSettingsEntity entity) => throw new NotImplementedException();

  public override Task<DalResult> DeleteAsync(int id) => throw new NotImplementedException();
}
