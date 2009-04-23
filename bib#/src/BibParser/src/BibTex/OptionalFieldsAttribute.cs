using System;

namespace EugenePetrenko.BibParser.BibTex
{
  public class OptionalFieldsAttribute : Attribute
  {
    private readonly BibField[] myField;

    public OptionalFieldsAttribute(params BibField[] field)
    {
      myField = field;
    }
  }
}