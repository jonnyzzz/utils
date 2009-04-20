using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EugenePetrenko.BibParser.AuxParser
{
  public class AuxReader
  {
    private readonly TextReader myReader;

    public AuxReader(TextReader reader)
    {
      myReader = reader;
    }

    
    public HashSet<string> ReadRefs()
    {
      var result = new HashSet<string>();
      string s;
      while (( s= myReader.ReadLine()) != null)
      {
        var START = @"\citation{";
        if (s.StartsWith(START))
          result.UnionWith(s.Substring(START.Length).Trim().TrimEnd('}').Split(',').Select(x => x.Trim()));
      }
      return result;
    }
  }
}