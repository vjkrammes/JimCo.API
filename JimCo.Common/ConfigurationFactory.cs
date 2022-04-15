
using JimCo.Common.Interfaces;

using Microsoft.Extensions.Configuration;

namespace JimCo.Common;
public class ConfigurationFactory : IConfigurationFactory
{
  public IConfiguration Create(string filename, bool isOptional = true, string? directory = null)
  {
    var ret = new ConfigurationBuilder()
      .SetBasePath(string.IsNullOrWhiteSpace(directory) ? Directory.GetCurrentDirectory() : directory)
      .AddJsonFile(filename, optional: isOptional, reloadOnChange: true)
      .AddEnvironmentVariables()
      .Build();
    return ret;
  }
}
