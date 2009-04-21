using System;
using BibParser;

namespace EugenePetrenko.BibParser.BibTex
{
  public class RequiredFieldsAttribute : Attribute
  {
    private readonly BibField[] myField;

    public RequiredFieldsAttribute(params BibField[] field)
    {
      myField = field;
    }
  }
}