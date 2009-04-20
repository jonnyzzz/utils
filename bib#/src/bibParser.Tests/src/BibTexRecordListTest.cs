using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace EugenePetrenko.BibParser.Tests
{
  [TestFixture]
  public class BibTexRecordListTest : BibTextReaderTestBase
  {
    [Test]
    public void Test_1()
    {
      ParseRecordsTest("@ZZZ{ddd,a=b}", 1);
    }
    
    [Test]
    public void Test_0()
    {
      ParseRecordsTest("", 0);
    }

    [Test]
    public void Test_2()
    {
      ParseRecordsTest("@ZZZ{ddd,a=b}@ZZZ{ddd,a=b}", 2);
    }

    [Test]
    public void Test_3()
    {
      ParseRecordsTest("@ZZZ{ddd,a=b}\r\n@ZZZ{ddd,a=b}  @ZZZ{ddd,a=b}", 3);
    }

    [Test]
    public void Test_JABREfComment()
    {
      ParseRecordsTest("@comment{jabref-meta: selector_keywords:}", 0);
    }
    
    private void ParseRecordsTest(string code, int count)
    {
      var data = CreateParser(code).Parse();

      Assert.That(data.Count, Is.EqualTo(count));
    }
  }
}