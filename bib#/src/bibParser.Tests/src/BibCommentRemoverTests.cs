using System.IO;
using EugenePetrenko.BibParser.Reader;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace EugenePetrenko.BibParser.Tests
{
  [TestFixture]
  public class BibCommentRemoverTests
  {
    [Test]
    public void Empty()
    {
      DoText("", "");
    }

    [Test]
    public void FullComment()
    {
      DoText("%", "");
    }

    [Test]
    public void FullComment2()
    {
      DoText("%RRR", "");
    }

    [Test]
    public void TextAndComment()
    {
      DoText("ZZZ%XXX", "ZZZ");
    }
    
    [Test]
    public void TextAndComment2()
    {
      DoText("ZZZ%XXX\r\nRRR", "ZZZ\r\nRRR");
    }

    private static void DoText(string input, string result)
    {
      var sw = new StringReader(input);
      var r = new BibCommentRemover(sw);
      Assert.That(r.ReadToEnd().TrimEnd(), Is.EqualTo(result));
    }
  }
}