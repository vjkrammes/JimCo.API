
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface ILogRepository : IRepository<LogEntity>
{
  Task<IEnumerable<LogEntity>> GetForDateAsync(DateTime date, Level level = Level.NoLevel);
  Task<IEnumerable<DateTime>> GetDatesAsync();
}
