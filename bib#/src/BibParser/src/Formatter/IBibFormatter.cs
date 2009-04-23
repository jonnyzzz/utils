using System;
using EugenePetrenko.BibParser.BibTex;
using EugenePetrenko.BibParser.Reader;

namespace EugenePetrenko.BibParser.Formatter
{
  public interface IBibFormatter
  {
    bool Matches(BibRecord record);

    FormattedRefernce Format(BibRecord record);
  }
}