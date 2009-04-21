using System;
using EugenePetrenko.BibParser.Reader;
using System.Linq;

namespace EugenePetrenko.BibParser.BibTex
{
  public enum BibRecordType
  {
    [RequiredFields(BibField.Author, BibField.Journal, BibField.Pages, BibField.Title, BibField.Volume, BibField.Year)]
    [OptionalFields(BibField.Language)]
    ARTICLE

  }

  public enum BibField
  {  
    Author, 
    Title,
    Journal,
    Year,
    Volume,
    Pages,
    Language,
  }

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


  public interface IBibRecord
  {
    string[] Authors { get; }
    
  }

  public class BibRecord //: IBibRecord
  {
    private readonly RawRecord myData;

    public BibRecord(RawRecord data)
    {
      myData = data;
    }

    private static FieldNameAttribute GetByField(BibField field)
    {
      var enumType = field.GetType();
      var fieldName = Enum.GetName(enumType, field);
      return (FieldNameAttribute) enumType.GetField(fieldName).GetCustomAttributes(typeof (FieldNameAttribute), true)[0];
    }

    private string GetKey(BibField field)
    {
      return
        myData.Pairs
        .Where(x => x.First.Equals(GetByField(field).Name, StringComparison.InvariantCultureIgnoreCase))
        .Single().Second;
    }
  }
}