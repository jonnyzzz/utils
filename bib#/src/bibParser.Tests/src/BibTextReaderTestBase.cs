using System.IO;
using EugenePetrenko.BibParser.Reader;

namespace EugenePetrenko.BibParser.Tests
{
  public abstract class BibTextReaderTestBase
  {
    protected BibReader CreateParser(string code)
    {
      return new BibReader(new BibLexer(new StringReader(code)));
    } 
  }
}