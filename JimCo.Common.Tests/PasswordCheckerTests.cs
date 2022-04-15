
using JimCo.Common.Enumerations;
using JimCo.Common.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JimCo.Common.Tests;

[TestClass]
public class PasswordCheckerTests
{
  private readonly IPasswordChecker _checker = new PasswordChecker();

  [TestMethod]
  public void TestPasswordChecker()
  {
    var ret = _checker.GetPasswordStrength("password");
    Assert.AreEqual(PasswordStrength.Weak, ret);
    ret = _checker.GetPasswordStrength("B00g3r!@#");
    Assert.AreEqual(PasswordStrength.VeryStrong, ret);
    ret = _checker.GetPasswordStrength(string.Empty);
    Assert.AreEqual(PasswordStrength.Blank, ret);
  }

  [TestMethod]
  public void TestPasswordZxcvbn()
  {
    var ret = _checker.GetPasswordStrengthZxcvbn("password", out var _);
    Assert.AreEqual(PasswordStrength.VeryWeak, ret);
    ret = _checker.GetPasswordStrengthZxcvbn("B00g3r!@#", out var _);
    Assert.AreEqual(PasswordStrength.Medium, ret);
    ret = _checker.GetPasswordStrengthZxcvbn("A|G'rJ!Hj.MgVLl", out var _);
    Assert.AreEqual(PasswordStrength.VeryStrong, ret);
    ret = _checker.GetPasswordStrengthZxcvbn(string.Empty, out var _);
    Assert.AreEqual(PasswordStrength.Blank, ret);
  }

  [TestMethod]
  public void TestGeneratePassword()
  {
    var password = _checker.GeneratePassword(12);
    Assert.AreEqual(12, password.Length);
    Assert.AreEqual(PasswordStrength.VeryStrong, _checker.GetPasswordStrengthZxcvbn(password, out var _));
  }
}
