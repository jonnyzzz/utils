using System;
using System.Collections.Generic;
using System.Linq;
using EugenePetrenko.BibParser.Reader;
using EugenePetrenko.BibParser.Util;

namespace EugenePetrenko.BibParser.BibTex
{
  public class BibRecord
  {
    private readonly RawRecord myData;

    public BibRecord(RawRecord data)
    {
      myData = data;
    }

    private string GetKey(BibField field)
    {
      return
        myData.Pairs
          .Where(
          x => x.First.Equals(field.GetByField<FieldNameAttribute>().Name, StringComparison.InvariantCultureIgnoreCase))
          .Single().Second;
    }

    public IEnumerable<BibAuthor> Authors
    {
      get
      {
        return GetKey(BibField.Author)
          .Split(new[] {"and"}, StringSplitOptions.RemoveEmptyEntries)
          .Select(x => new BibAuthor(x.Trim()));
      }
    }

    public string Title
    {
      get { return GetKey(BibField.Title); }
    }

    public string RefName
    {
      get { return myData.RefName; }
    }

    public BibRecordType Type
    {
      get
      {
        string type = myData.Type;
        foreach (BibRecordType value in Enum.GetValues(typeof(BibRecordType)))
        {
          if (type.Equals(value.GetByField<FieldNameAttribute>().Name, StringComparison.InvariantCultureIgnoreCase))
          {
            return value;
          }
        }
        throw new ParseException(string.Format("Unable to find '{0}' record type", type));
      }
    }

    public string Journal
    {
      get { return GetKey(BibField.Journal); }
    }

    public string Pages
    {
      get { return GetKey(BibField.Pages); }
    }

    public string Year
    {
      get { return GetKey(BibField.Year); }
    }

    public string Number
    {
      get { return GetKey(BibField.Number); }
    }

    public string Volume
    {
      get { return GetKey(BibField.Volume); }
    }

    public string URL
    {
      get { return GetKey(BibField.Url); }
    }

    public bool IsRussian()
    {
      throw new NotImplementedException();
    }
  }
}