using System;
using BibParser;

namespace EugenePetrenko.BibParser.BibTex
{
  public class OptionalFieldsAttribute : Attribute
  {
    private readonly BibRecordFields[] myFields;

    public OptionalFieldsAttribute(params BibRecordFields[] fields)
    {
      myFields = fields;
    }
  }
}