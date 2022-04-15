
using JimCo.Common.Enumerations;

using Zxcvbn;

namespace JimCo.Common.Interfaces;
public interface IPasswordChecker
{
  PasswordStrength GetPasswordStrength(string password);
  PasswordStrength GetPasswordStrengthZxcvbn(string password, out Result result);
  string GeneratePassword(int length);
  bool IsStrongPassword(string password);
  bool IsValidPassword(string password, PasswordOptions options, bool requireStrongPassword = false);
  bool IsValidPassword(string password, int minlen, int minunique, bool specials, bool lower, bool upper, bool digits, bool reqiureStrongPassword = false);
  string[] ValidatePassword(string password, PasswordOptions options, bool requireStrongPassword = false);
  bool HasMinimumLength(string password, int length);
  bool HasMinimumUniqueCharacters(string password, int count);
  bool HasDigit(string password);
  bool HasSpecialCharacter(string password);
  bool HasLowercaseLetter(string password);
  bool HasUppercaseLetter(string password);
}
