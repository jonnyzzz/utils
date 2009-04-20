using System;
using BibParser;

namespace EugenePetrenko.BibParser.BibTex
{
  public class RequiredFieldsAttribute : Attribute
  {
    private readonly BibRecordFields[] myFields;

    public RequiredFieldsAttribute(params BibRecordFields[] fields)
    {
      myFields = fields;
    }
  }
}