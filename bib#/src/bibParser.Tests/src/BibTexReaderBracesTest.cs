using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace EugenePetrenko.BibParser.Tests
{
  [TestFixture]
  public class BibTexReaderBracesTest : BibTextReaderTestBase
  {
    private void DoBracesTest(string code, string result)
    {
      var data = CreateParser(code).ParseBraces();
      Assert.That(data, Is.EqualTo(result));
    }

    [Test]
    public void Braces_Simple()
    {
      DoBracesTest("abc", "abc");
    }

    [Test]
    public void Braces_Trim()
    {
      DoBracesTest(" abc ", "abc");
    }

    [Test]
    public void Braces_Trim_Multi_Spaces_Inside()
    {
      DoBracesTest(" a   b   c ", "a b c");
    }

    [Test]
    public void Braces_Trim_Multi_Spaces_Inside_2()
    {
      DoBracesTest(" a   b c ", "a b c");
    }

    [Test]
    public void Braces_Trim_NewLine()
    {
      DoBracesTest(" a  \r\n b c ", "a b c");
    }

    [Test]
    public void Braces_Trim_Tab()
    {
      DoBracesTest(" a  \t b c ", "a b c");
    }

    [Test]
    public void Braces_Brace_L1()
    {
      DoBracesTest("{abc}", "abc");
    }

    [Test]
    public void Braces_Brace_L1_Spaces()
    {
      DoBracesTest(" { abc } ", "abc");
    }

    [Test]
    public void Braces_Brace_L2()
    {
      DoBracesTest("{{a}b{c}} ", "{a}b{c}");
    }

  }
}