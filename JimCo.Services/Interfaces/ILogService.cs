
using JimCo.Common.Enumerations;
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface ILogService : IDataService<LogModel>
{
  Task<IEnumerable<LogModel>> GetForDateAsync(DateTime date, Level level = Level.NoLevel);
  Task<IEnumerable<DateTime>> GetDatesAsync();
}
