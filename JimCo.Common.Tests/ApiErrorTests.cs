
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JimCo.Common.Tests;

[TestClass]
public class ApiErrorTests
{
  [TestMethod]
  public void TestApiError()
  {
    var error = ApiError.FromDalResult(DalResult.Success);
    Assert.IsTrue(error.Successful);
  }
}
