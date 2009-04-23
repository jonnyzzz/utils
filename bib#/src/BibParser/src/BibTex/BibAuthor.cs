namespace EugenePetrenko.BibParser.BibTex
{
  public class BibAuthor
  {
    private readonly string myName;

    public BibAuthor(string name)
    {
      myName = name;
    }

    public string Name
    {
      get { return myName; }
    }

    public string ToTex()
    {
      return Name;
    }
  }
}