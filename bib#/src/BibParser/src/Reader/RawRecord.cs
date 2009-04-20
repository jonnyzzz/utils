using System.Collections.Generic;
using EugenePetrenko.BibParser.Util;

namespace EugenePetrenko.BibParser.Reader
{
  public class RawRecord
  {
    public readonly string RefName;
    public readonly string Type;
    public ICollection<Pair<string, string>> Pairs;

    public RawRecord(string refName, string type, ICollection<Pair<string, string>> pairs)
    {
      RefName = refName;
      Type = type;
      Pairs = pairs;
    }
  }
}