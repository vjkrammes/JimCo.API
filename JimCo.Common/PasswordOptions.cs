namespace JimCo.Common;
public sealed class PasswordOptions
{
  public int MinimumLength { get; set; }
  public int MinimumUniqueCharacters { get; set; }
  public bool RequireUppercase { get; set; }
  public bool RequireLowercase { get; set; }
  public bool RequireDigit { get; set; }
  public bool RequireSpecialCharacter { get; set; }

  public PasswordOptions()
  {
    MinimumLength = 8;
    MinimumUniqueCharacters = 4;
    RequireUppercase = true;
    RequireLowercase = true;
    RequireDigit = true;
    RequireSpecialCharacter = true;
  }

  public static PasswordOptions Default => new();
}
