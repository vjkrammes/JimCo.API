using System.Security.Cryptography;
using System.Text;

using JimCo.Common.Enumerations;
using JimCo.Common.Interfaces;

using Zxcvbn;

namespace JimCo.Common;
public class PasswordChecker : IPasswordChecker
{
  public PasswordStrength GetPasswordStrength(string password)
  {
    var score = 0;
    if (string.IsNullOrWhiteSpace(password))
    {
      return PasswordStrength.Blank;
    }
    if (!HasMinimumLength(password, 5))
    {
      return PasswordStrength.VeryWeak;
    }
    score++;
    if (HasMinimumLength(password, 8))
    {
      score++;
    }
    if (HasUppercaseLetter(password) && HasLowercaseLetter(password))
    {
      score++;
    }
    if (HasDigit(password))
    {
      score++;
    }
    if (HasSpecialCharacter(password))
    {
      score++;
    }
    return (PasswordStrength)score;
  }

  public PasswordStrength GetPasswordStrengthZxcvbn(string password, out Result result)
  {
    if (string.IsNullOrWhiteSpace(password))
    {
      result = null!;
      return PasswordStrength.Blank;
    }
    result = Zxcvbn.Core.EvaluatePassword(password);
    return (PasswordStrength)(result.Score + 1);
  }

  private static string MakePassword(int length)
  {
    const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$^*()_-=[]/`|";
    var len = characters.Length;
    var sb = new StringBuilder();
    using var rand = RandomNumberGenerator.Create();
    var rndbytes = new byte[4];
    for (var i = 0; i < length; i++)
    {
      rand.GetBytes(rndbytes);
      var r = BitConverter.ToInt32(rndbytes, 0);
      sb.Append(characters[Math.Abs(r) % len]);
    }
    return sb.ToString();
  }

  public string GeneratePassword(int length)
  {
    var ret = MakePassword(length);
    var strength = GetPasswordStrengthZxcvbn(ret, out var _);
    while (strength != PasswordStrength.VeryStrong)
    {
      ret = MakePassword(length);
      strength = GetPasswordStrengthZxcvbn(ret, out var _);
    }
    return ret;
  }

  public bool IsStrongPassword(string password) =>
      GetPasswordStrengthZxcvbn(password, out var _) == PasswordStrength.VeryStrong;

  public bool IsValidPassword(string password, PasswordOptions options, bool requireStrongPassword = false) => options is not null &&
      IsValidPassword(password, options.MinimumLength, options.MinimumUniqueCharacters, options.RequireSpecialCharacter,
          options.RequireLowercase, options.RequireUppercase, options.RequireDigit, requireStrongPassword);

  public bool IsValidPassword(string password, int minlen, int minunique, bool special, bool lower, bool upper, bool digits,
      bool requireStrongPassword = false)
  {
    if (!HasMinimumLength(password, minlen))
    {
      return false;
    }
    if (!HasMinimumUniqueCharacters(password, minunique))
    {
      return false;
    }
    if (special && !HasSpecialCharacter(password))
    {
      return false;
    }
    if (lower && !HasLowercaseLetter(password))
    {
      return false;
    }
    if (upper && !HasUppercaseLetter(password))
    {
      return false;
    }
    if (digits && !HasDigit(password))
    {
      return false;
    }
    return !requireStrongPassword || GetPasswordStrengthZxcvbn(password, out var _).IsStrongPassword();
  }

  public string[] ValidatePassword(string password, PasswordOptions options, bool requireStrongPassword = false)
  {
    if (options is null)
    {
      throw new ArgumentNullException(nameof(options));
    }
    var errors = new List<string>();
    if (!HasMinimumLength(password, options.MinimumLength))
    {
      errors.Add($"Password is too short, the minimum length is {options.MinimumLength} character(s)");
    }
    if (!HasMinimumUniqueCharacters(password, options.MinimumUniqueCharacters))
    {
      errors.Add($"Password must contain at least {options.MinimumUniqueCharacters} unique character(s)");
    }
    if (options.RequireSpecialCharacter && !HasSpecialCharacter(password))
    {
      errors.Add("At least one special character is required");
    }
    if (options.RequireLowercase && !HasLowercaseLetter(password))
    {
      errors.Add("At least one lowercase leter is required");
    }
    if (options.RequireUppercase && !HasUppercaseLetter(password))
    {
      errors.Add("At least one uppercase letter is required");
    }
    if (options.RequireDigit && !HasDigit(password))
    {
      errors.Add("At least one digit is required");
    }
    var strength = GetPasswordStrengthZxcvbn(password, out var _);
    if (requireStrongPassword && !strength.IsStrongPassword())
    {
      errors.Add($"Strong password is required, this password is {strength.GetDescriptionFromEnumValue()}");
    }
    return errors.ToArray();
  }

  #region Has... Methods

  public bool HasMinimumLength(string password, int length) => !string.IsNullOrEmpty(password) && password.Length >= length;

  public bool HasMinimumUniqueCharacters(string password, int count) =>
      !string.IsNullOrEmpty(password) && password.Distinct().Count() >= count;

  public bool HasSpecialCharacter(string password) =>
      !string.IsNullOrEmpty(password) && password.IndexOfAny("!@#$%^&*()_+-={}[];:'\"<>,./?|\\~`".ToCharArray()) != -1;

  public bool HasDigit(string password) => !string.IsNullOrEmpty(password) && password.Any(x => char.IsDigit(x));

  public bool HasLowercaseLetter(string password) => !string.IsNullOrEmpty(password) && password.Any(x => char.IsLower(x));
  public bool HasUppercaseLetter(string password) => !string.IsNullOrEmpty(password) && password.Any(x => char.IsUpper(x));

  #endregion
}
