using System.Linq;
using System.Text;
using EugenePetrenko.BibParser.BibTex;
using EugenePetrenko.BibParser.Util;

namespace EugenePetrenko.BibParser.Formatter
{
  public class ArticleFormatter : IBibFormatter
  {
    public bool Matches(BibRecord record)
    {
      return record.Type == BibRecordType.ARTICLE;
    }

    public FormattedRefernce Format(BibRecord record)
    {
      string key = record.RefName;
      
      var sb = new StringBuilder();

      sb.Append(record.Authors.Select(x => x.ToTex()).JoinString(", "));
      sb.Append(" ");
      sb.Append(record.Title);
      sb.Append(" // ");
      sb.Append(record.Journal);
      sb.Append(". ");
      sb.Append(record.Year);
      sb.Append(". ");

      if (record.Number != null)
        sb.Append(record.IsRussian()? "¹ " : "No ").Append(record.Number);
      if (record.Number != null && record.Volume != null)
        sb.Append(", ");
      if (record.Volume != null)
        sb.Append(record.IsRussian()? "T " : "Vol ").Append(record.Volume);
 
      sb.Append(". ");
      sb.Append(record.IsRussian() ? "Ñ. " : "P. ").Append(record.Pages);

      if (record.URL != null)
        sb.Append("URL: ").Append(record.URL);

      return new FormattedRefernce(key, sb.ToString());
    }
  }
}