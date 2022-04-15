
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JimCo.Common.Tests;

[TestClass]
public class SkuGeneratorTests
{
  [TestMethod]
  public void TestSkuGenerator()
  {
    var generator = new SkuGenerator();
    Assert.IsNotNull(generator);
    var result = generator.GenerateSku(10);
    Assert.IsTrue(result.Length == 10);
    for (var i = 0; i < result.Length; i++)
    {
      if (i == 0)
      {
        Assert.IsTrue(result[i] is > '0' and <= '9');
      }
      else
      {
        Assert.IsTrue(result[i] is >= '0' and <= '9');
      }
    }
  }
}
