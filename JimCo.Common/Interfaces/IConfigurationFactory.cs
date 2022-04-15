
using Microsoft.Extensions.Configuration;

namespace JimCo.Common.Interfaces;
public interface IConfigurationFactory
{
  IConfiguration Create(string filename, bool isOptional, string? directory = null);
}
