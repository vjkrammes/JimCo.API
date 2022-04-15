using System.Text;

using JimCo.Common.Interfaces;

namespace JimCo.Common;
public class SkuGenerator : ISkuGenerator
{
  private static readonly char[] _digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
  public string GenerateSku(int length = 10)
  {
    var sb = new StringBuilder();
    var random = new Random();
    for (var i = 0; i < length; i++)
    {
      if (i == 0) // first digit is non-zero
      {
        sb.Append(_digits[random.Next(9) + 1]);
      }
      else
      {
        sb.Append(_digits[random.Next(10)]);
      }
    }
    return sb.ToString();
  }
}
