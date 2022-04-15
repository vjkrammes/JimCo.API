
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JimCo.Common.Tests;

[TestClass]
public class ApiResultTests
{
  private readonly string _successValue = "If it weren't for my horse, I never would have spent that year in college.";
  private readonly int _failureValue = 42;

  [TestMethod]
  [ExpectedException(typeof(NotFailureException))]
  public void TestSuccessResult()
  {
    var success = new ApiResult<string, int>(_successValue);
    Assert.IsTrue(success.IsSuccessResult);
    Assert.AreEqual(success.SuccessPayload, _successValue);
    _ = success.FailurePayload;
  }

  [TestMethod]
  [ExpectedException(typeof(NotSuccessException))]
  public void TestFailureResult()
  {
    var failure = new ApiResult<string, int>(_failureValue);
    Assert.IsFalse(failure.IsSuccessResult);
    Assert.AreEqual(failure.FailurePayload, _failureValue);
    _ = failure.SuccessPayload;
  }
}
