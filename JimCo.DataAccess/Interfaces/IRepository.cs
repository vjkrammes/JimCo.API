
using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess.Interfaces;
public interface IRepository<TEntity> where TEntity : class, IIdEntity, ISqlEntity, new()
{
  Task<int> CountAsync();
  Task<DalResult> InsertAsync(TEntity entity);
  Task<DalResult> UpdateAsync(TEntity entity);
  Task<DalResult> DeleteAsync(TEntity entity);
  Task<DalResult> DeleteAsync(int id);
  Task<IEnumerable<TEntity>> GetAsync();
  Task<IEnumerable<TEntity>> GetAsync(string sql, params QueryParameter[] parameters);
  Task<TEntity?> ReadAsync(int id);
  Task<TEntity?> ReadAsync(string sql, params QueryParameter[] parameters);
}
