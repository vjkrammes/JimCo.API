
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JimCo.Common.Tests;

[TestClass]
public class IdEncoderTests
{
  private readonly int _id = 12345;

  [TestMethod]
  public void TestIdEncoder()
  {
    var hash = IdEncoder.EncodeId(_id);
    System.Console.WriteLine($"Hash = {hash}");
    Assert.IsFalse(string.IsNullOrWhiteSpace(hash));
    Assert.IsTrue(hash.Length >= 20);
    var decodedid = IdEncoder.DecodeId(hash);
    Assert.IsTrue(decodedid != 0);
    Assert.AreEqual(_id, decodedid);
  }
}
