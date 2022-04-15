using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JimCo.Common.Tests;

[TestClass]
public class LoremIpsumGeneratorTests
{
  [TestMethod]
  public void TestLoremIpsum()
  {
    var loremIpsum = new LoremIpsumGenerator();
    Assert.IsTrue(loremIpsum is not null);

    // test fixed numbers of words, and sentences

    var result = loremIpsum!.Generate(20, 20, 2, 2, 1);
    Assert.IsTrue(Regex.Matches(result, @"\. ").Count == 2); // 2 sentences
    var sentence1 = result.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).First()?.Trim();
    Assert.IsTrue(sentence1 is not null);
    var words = sentence1!.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    Assert.IsTrue(words.Length == 20);

    // test variable numbers of words and sentences

    var newresult = loremIpsum.Generate(10, 20, 2, 4, 1);
    var numsentences = Regex.Matches(newresult, @"\.").Count;
    Assert.IsTrue(numsentences is >= 2 and <= 4);
    var newsentence1 = newresult.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).First()?.Trim();
    Assert.IsTrue(newsentence1 is not null);
    var newwords = newsentence1!.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    var numwords = newwords.Length;
    Assert.IsTrue(numwords is >= 10 and <= 20);
  }
}
