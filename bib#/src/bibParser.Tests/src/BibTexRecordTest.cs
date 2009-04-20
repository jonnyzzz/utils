using System;
using System.Linq;
using EugenePetrenko.BibParser.Util;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace EugenePetrenko.BibParser.Tests
{
  [TestFixture]
  public class BibTexRecordTest : BibTextReaderTestBase
  {

    [Test]
    public void Record_Simple()
    {
      ParseRecordeTest("@ARTICLE{zzz,a=b}", "ARTICLE", "zzz", "a=b");
    }

    [Test]
    public void Record_Spaces()
    {
      ParseRecordeTest("@ARTICLE{zzz, \r\n   a=b   ,   \r\n c=d     }", "ARTICLE", "zzz", "a=b", "c=d");
    }

    [Test]
    public void Record_Simple_NewLine()
    {
      ParseRecordeTest("@ARTICLE{zzz,a=b\r\n}", "ARTICLE", "zzz", "a=b");
    }

    [Test]
    public void Record_Simple_NewLine2()
    {
      ParseRecordeTest("@ARTICLE{zzz,a={b}\r\n}", "ARTICLE", "zzz", "a=b");
    }
    
    [Test]
    public void Record_2_argz()
    {
      ParseRecordeTest("@ARTICLE{zzz,a=b,c=d}", "ARTICLE", "zzz", "a=b", "c=d");
    }

    [Test]
    public void Record_complex_argz()
    {
      ParseRecordeTest("@ARTICLE{zzz,a={b,c=d}}", "ARTICLE", "zzz", "a=b,c=d");
    }

    [Test]
    public void Record_complex_argz_2()
    {
      ParseRecordeTest("@ARTICLE{zzz,a={b,c=d},e=f}", "ARTICLE", "zzz", "a=b,c=d", "e=f");
    }
 
    private void ParseRecordeTest(string code, string type, string @ref, params string[] pairs)
    {
      var data = CreateParser(code).ParseBibRecord();

      Assert.That(data.Type, Is.EqualTo(type));
      Assert.That(data.RefName, Is.EqualTo(@ref));

      Assert.That(
        data.Pairs.Select(x => x.First + "=" + x.Second).JoinString(Environment.NewLine), 
        Is.EqualTo(pairs.JoinString(Environment.NewLine)));
    }

   
  }
}