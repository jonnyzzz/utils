namespace EugenePetrenko.BibParser.Formatter
{
  public class FormattedRefernce
  {
    public string RefenceName { get; private set; }
    public string TexSource { get; private set; }

    public FormattedRefernce(string refenceName, string texSource)
    {
      RefenceName = refenceName;
      TexSource = texSource;
    }
  }
}