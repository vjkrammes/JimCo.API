
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess.Interfaces;
public interface IDatabaseBuilder
{
  Task BuildDatabaseAsync(bool dropIfExists);
  (int order, string sql)[] Tables();
  (int order, string name)[] TableNames();
  (int order, List<IndexDefinition> indices)[] Indices();
}
