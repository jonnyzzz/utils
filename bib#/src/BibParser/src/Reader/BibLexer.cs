using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EugenePetrenko.BibParser.Util;

namespace EugenePetrenko.BibParser.Reader
{
  public class BibLexer
  {
    private readonly TextReader myReader;

    public BibLexer(TextReader reader)
    {
      myReader = reader;
    }

    public void ReadAllWritespaces()
    {
      while (char.IsWhiteSpace((char)myReader.Peek()))
      {
        myReader.Read();
      }
    }
    
    public char AssertChar(params char[] c)
    {
      var peek = myReader.Peek();
      if (!c.Contains((char)peek))
      {
        if (peek == -1)
          throw new ParseException("Expected '" + c.JoinString(", ") + "' but was end of file");

        throw new ParseException("Expected '" + c.JoinString(", ") + "' but was '" + (char)myReader.Read() + "'");
      }

      return (char) myReader.Read();
    }

    public bool IsNext(char c)
    {
      return myReader.Peek() == c;
    }

    public string ReadUntil(params char[] cc)
    {
      return ReadUntil(cc, false);
    }

    public string ReadUntilEnd(params char[] cc)
    {
      return ReadUntil(cc, true);
    }

    private string ReadUntil(char[] cc, bool stopOnEnd)
    {
      var ends = new HashSet<char>(cc);
      var sb = new StringBuilder();
      while (!ends.Contains((char)myReader.Peek()))
      {
        var ch = myReader.Read();
        if (ch == -1)
        {
          if (!stopOnEnd)
            throw new ParseException("Unexpected end of file"); 
          break;
        }
        sb.Append((char)ch);
      }
      return sb.ToString();
    }
  }
}