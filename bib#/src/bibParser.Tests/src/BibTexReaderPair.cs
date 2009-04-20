using NUnit.Framework;

namespace EugenePetrenko.BibParser.Tests
{
  [TestFixture]
  public class BibTexReaderPair : BibTextReaderTestBase
  {
    [Test]
    public void Pair_emptry()
    {
      DoParsePairTest("=", "", "");
    }

    [Test]
    public void Pair_Simple()
    {
      DoParsePairTest("a=b", "a", "b");
    }

    [Test]
    public void Pair_Braces()
    {
      DoParsePairTest("a={b,c=d}", "a", "b,c=d");
    }

    [Test]
    public void Pair_DoubleBraces()
    {
      DoParsePairTest("a={b,{\"c} {{{sss}s}d}}", "a", "b,{\"c} {{{sss}s}d}");
    }

    private void DoParsePairTest(string code, string key, string value)
    {
      var pair = CreateParser(code).ParsePair();
      Assert.AreEqual(key, pair.First);
      Assert.AreEqual(value, pair.Second);
    }
  }
}