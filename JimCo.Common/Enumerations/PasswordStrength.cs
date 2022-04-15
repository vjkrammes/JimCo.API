using System.ComponentModel;

namespace JimCo.Common.Enumerations;
public enum PasswordStrength
{
  [Description("Blank")]
  Blank = 0,
  [Description("Very Weak")]
  VeryWeak = 1,
  [Description("Weak")]
  Weak = 2,
  [Description("Medium")]
  Medium = 3,
  [Description("Strong")]
  Strong = 4,
  [Description("Very Strong")]
  VeryStrong = 5
}
