using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EugenePetrenko.BibParser.Util;

namespace EugenePetrenko.BibParser.Reader
{
  public class BibReader
  {
    private readonly BibLexer myLex;

    public BibReader(BibLexer lex)
    {
      myLex = lex;
    }

    public ICollection<RawRecord> Parse()
    {
      var data = new List<RawRecord>();
      myLex.ReadAllWritespaces();
      if (myLex.IsNext('@'))
      {
        data.Add(ParseBibRecord());
      }
      return data;
    }

    public RawRecord ParseBibRecord()
    {
      string type = ParseArticleType();
      myLex.AssertChar('{');
      string refName = myLex.ReadUntil(',').Trim();
      myLex.AssertChar(',');

      var pairs = ParsePairs().ToList();

      myLex.AssertChar('}');

      myLex.ReadAllWritespaces();
      return new RawRecord(refName, type, pairs);
    }

    public string ParseArticleType()
    {
      myLex.ReadAllWritespaces();

      myLex.AssertChar('@');

      return myLex.ReadUntil('{');
    }

    private IEnumerable<Pair<string, string>> ParsePairs()
    {
      var data = new List<Pair<string, string>>();
      
      data.Add(ParsePair());
      while (myLex.IsNext(','))
      {
        myLex.AssertChar(',');
        data.Add(ParsePair());
      }
      return data;
    }

    public Pair<string, string> ParsePair()
    {
      string key = ParseBraces('=');
      myLex.AssertChar('=');
      string value = ParseBraces(',', '}').Trim();

      return Pair.Of(key, value);
    } 

    private static readonly Regex SPACES = new Regex(@"\s+");
    public string ParseBraces(params char[] teminators)
    {
      return SPACES.Replace(DoParseBraces(teminators).Trim(), " ");
    }

    private string DoParseBraces(char[] teminators)
    {
      myLex.ReadAllWritespaces();

      if (!myLex.IsNext('{'))
        return myLex.ReadUntilEnd(teminators);

      int balance = 0;
      var data = new StringBuilder();
      myLex.AssertChar('{');
      balance++;
       
      while (true)
      {
        data.Append(myLex.ReadUntilEnd('{', '}'));

        char c = myLex.AssertChar('{', '}');

        if (balance == 1 && c == '}')
        {
          return data.ToString();
        }
        data.Append(c);

        if (c == '}') balance--;
        if (c == '{') balance++;

        if (balance < 1)
        {
          throw new ParseException("Wrong braces balance");
        }
      }
    }
  }
}