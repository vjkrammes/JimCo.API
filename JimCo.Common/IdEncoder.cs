using HashidsNet;

using JimCo.Common.Interfaces;

namespace JimCo.Common;
public static class IdEncoder
{
  private readonly static string _salt;
  private readonly static IHashids _hasher;

  static IdEncoder()
  {
    IConfigurationFactory factory = new ConfigurationFactory();
    var config = factory.Create(Constants.ConfigurationFilename, isOptional: false);
    _salt = config["IdEncoderSalt"];
    if (string.IsNullOrWhiteSpace(_salt))
    {
      _salt = Constants.DefaultIdEncoderSalt;
    }
    _hasher = new Hashids(_salt, 20);
  }

  public static string EncodeId(int id) => _hasher.Encode(id);

  public static int DecodeId(string hash) => _hasher.Decode(hash)?.FirstOrDefault() ?? 0;
}
