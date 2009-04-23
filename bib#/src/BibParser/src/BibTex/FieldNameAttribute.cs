using System;

namespace EugenePetrenko.BibParser.BibTex
{
  public class FieldNameAttribute : Attribute
  {
    private readonly string myName;

    public FieldNameAttribute(string name)
    {
      myName = name;
    }

    public string Name
    {
      get { return myName; }
    }
  }
}