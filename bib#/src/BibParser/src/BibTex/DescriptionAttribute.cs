using System;

namespace EugenePetrenko.BibParser.BibTex
{
  public class DescriptionAttribute : Attribute
  {
    private readonly string myShort;
    private readonly string myDescription;

    public DescriptionAttribute(string @short, string description)
    {
      myShort = @short;
      myDescription = description;
    }
  }
}