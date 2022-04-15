using System.Reflection;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Interfaces;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

using Microsoft.Extensions.Configuration;

namespace JimCo.DataAccess;
public class DatabaseBuilder : IDatabaseBuilder
{
  private readonly Dictionary<int, string> _tables = new();
  private readonly Dictionary<int, string> _tableNames = new();
  private readonly Dictionary<int, List<IndexDefinition>> _indices = new();
  private readonly IDatabase _database;
  private readonly IConfiguration? _configuration;
  private readonly IAlertSeeder _alertSeeder;
  private readonly ICategorySeeder _categorySeeder;
  private readonly ILineItemSeeder _lineItemSeeder;
  private readonly IOrderSeeder _orderSeeder;
  private readonly IProductSeeder _productSeeder;
  private readonly IPromotionSeeder _promotionSeeder;
  private readonly ISystemSettingsSeeder _systemSettingsSeeder;
  private readonly IUserSeeder _userSeeder;
  private readonly IVendorSeeder _vendorSeeder;

  public DatabaseBuilder(IDatabase database, IConfiguration? configuration, IAlertSeeder alertSeeder, ICategorySeeder categorySeeder, ILineItemSeeder lineItemSeeder,
    IOrderSeeder orderSeeder, IProductSeeder productSeeder, IPromotionSeeder promotionSeeder, ISystemSettingsSeeder systemSettingsSeeder, IUserSeeder userSeeder, 
    IVendorSeeder vendorSeeder)
  {
    _database = database;
    _configuration = configuration;
    _alertSeeder = alertSeeder;
    _categorySeeder = categorySeeder;
    _lineItemSeeder = lineItemSeeder;
    _orderSeeder = orderSeeder;
    _productSeeder = productSeeder;
    _promotionSeeder = promotionSeeder;
    _systemSettingsSeeder = systemSettingsSeeder;
    _userSeeder = userSeeder;
    _vendorSeeder = vendorSeeder;
    LoadTables();
  }

  public async Task BuildDatabaseAsync(bool dropIfExists)
  {
    if (dropIfExists && _database.DatabaseExists())
    {
      _database.DropDatabase();
    }
    if (!_database.DatabaseExists())
    {
      _database.CreateDatabase();
    }
    if (_database.DatabaseExists())
    {
      _tables.OrderBy(x => x.Key).ForEach(x => _database.CreateTable(_tableNames[x.Key], x.Value));
      _indices
        .OrderBy(x => x.Key)
        .ForEach(x => _database.CreateIndices(_tableNames[x.Key], _indices[x.Key]));
      if (_configuration is not null)
      {
        await Seed();
      }
    }
  }

  public (int order, string sql)[] Tables()
  {
    List<(int, string)> ret = new();
    _tables.ForEach(x => ret.Add(new(x.Key, x.Value)));
    return ret.ToArray();
  }

  public (int order, string name)[] TableNames()
  {
    List<(int, string)> ret = new();
    _tableNames.ForEach(x => ret.Add(new(x.Key, x.Value)));
    return ret.ToArray();
  }

  public (int order, List<IndexDefinition> indices)[] Indices()
  {
    List<(int order, List<IndexDefinition> indices)> ret = new();
    _indices.ForEach(x => ret.Add(new(x.Key, x.Value)));
    return ret.ToArray();
  }

  private void LoadTables()
  {
    var assembly = Assembly.GetExecutingAssembly();
    var type = typeof(ISqlEntity);
    var types = assembly.GetTypes().Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
    if (types is not null && types.Any())
    {
      types.ForEach(x =>
      {
        var obj = Activator.CreateInstance(x);
        var sqlprop = x.GetProperty("Sql", BindingFlags.Public | BindingFlags.Static);
        if (sqlprop is not null)
        {
          var buildOrder = (x.GetCustomAttribute(typeof(BuildOrderAttribute), false) as BuildOrderAttribute)?.BuildOrder ?? 0;
          if (buildOrder > 0)
          {
            _indices[buildOrder] = new();
            if (_tableNames.ContainsKey(buildOrder))
            {
              throw new InvalidOperationException($"Multiple tables with build order '{buildOrder}'");
            }
            var sql = sqlprop.GetValue(obj) as string;
            if (sql is not null)
            {
              _tables.Add(buildOrder, sql);
            }
            var tableName = (x.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute)?.Name ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tableName))
            {
              throw new InvalidOperationException($"No table attribute found on class '{x.Name}'");
            }
            _tableNames.Add(buildOrder, tableName);
            var properties = x.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
              var attr = property.GetCustomAttribute(typeof(IndexedAttribute), false) as IndexedAttribute;
              if (attr is not null)
              {
                var index = new IndexDefinition
                {
                  ColumnName = property.Name,
                  IndexName = string.IsNullOrWhiteSpace(attr.IndexName) ? $"Ix{property.Name}" : attr.IndexName
                };
                _indices[buildOrder].Add(index);
              }
            }
          }
        }
      });
    }
  }

  private async Task Seed()
  {
    // no particular order except that Products must come after Categories and Vendors, and Promotions must come after Products

    if (_alertSeeder is not null)
    {
      await _alertSeeder.SeedAsync(_configuration!, "AlertSeeds");
    }
    if (_categorySeeder is not null)
    {
      await _categorySeeder.SeedAsync(_configuration!, "CategorySeeds");
    }
    if (_lineItemSeeder is not null)
    {
      await _lineItemSeeder.SeedAsync(_configuration!, "LineItemSeeds");
    }
    if (_orderSeeder is not null)
    {
      await _orderSeeder.SeedAsync(_configuration!, "OrderSeeds");
    }
    if (_userSeeder is not null)
    {
      await _userSeeder.SeedAsync(_configuration!, "UserSeeds");
    }
    if (_vendorSeeder is not null)
    {
      await _vendorSeeder.SeedAsync(_configuration!, "VendorSeeds");
    }
    if (_systemSettingsSeeder is not null)
    {
      await _systemSettingsSeeder.SeedAsync(_configuration!, "");
    }
    if (_productSeeder is not null)
    {
      await _productSeeder.SeedAsync(_configuration!, "ProductSeeds");
    }
    if (_promotionSeeder is not null)
    {
      await _promotionSeeder.SeedAsync(_configuration!, "PromotionSeeds");
    }
  }
}
