using System;
using System.Runtime.Serialization;
using EugenePetrenko.BibParser.Reader;
using EugenePetrenko.BibParser.Util;

namespace EugenePetrenko.BibParser.Formatter
{
  public interface IBibFormatter
  {
    bool Matches(RawRecord record);

    Pair<string, string> Format(RawRecord record);
  }

  public class ArticleFormatter : IBibFormatter
  {
    public bool Matches(RawRecord record)
    {
      return "article".Equals(record.Type, StringComparison.InvariantCultureIgnoreCase);
    }

    public Pair<string, string> Format(RawRecord record)
    {
      string key = record.RefName;
//      string value = 
      return null;
    }
  }
}