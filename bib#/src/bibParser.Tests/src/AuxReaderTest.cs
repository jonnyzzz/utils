using System;
using System.Collections.Generic;
using System.IO;
using EugenePetrenko.BibParser.AuxParser;
using EugenePetrenko.BibParser.Util;
using NUnit.Framework;
using System.Linq;
using NUnit.Framework.SyntaxHelpers;

namespace EugenePetrenko.BibParser.Tests
{
  [TestFixture]
  public class AuxReaderTest
  {
    [Test]
    public void Test_Empty()
    {
      DoTest("");
    }

    [Test]
    public void Test_Only()
    {
      DoTest(@"\citation{zzz}", "zzz");
    }

    [Test]
    public void Test_Multi()
    {
      DoTest(@"\citation{zzz,yyy}", "yyy","zzz");
    }

    [Test]
    public void Test_Multi_Others()
    {
      DoTest(@"1231321" + Environment.NewLine + @"\citation{zzz,yyy}", "yyy","zzz");
    }

    [Test]
    public void Test_Multi_Others2()
    {
      DoTest(@"\citation{zzz,yyy}" + Environment.NewLine + @"1231321", "yyy","zzz");
    }
    
    [Test]
    public void Test_Multi_Others_Same()
    {
      DoTest(@"\citation{zzz,yyy}" + Environment.NewLine + @"1231321" + "\r\n" + @"\citation{zzz,yyy}", "yyy", "zzz");
    }
    
    private void DoTest(string code, params string[] refs)
    {
      var expected = new HashSet<string>(refs);
      var actual = new AuxReader(new StringReader(code)).ReadRefs();

      Func<IEnumerable<string>, string> ToString = x => x.OrderBy(xx=>xx).JoinString(",");

      Assert.That(ToString(actual), Is.EqualTo(ToString(expected)));
    }
  }
}