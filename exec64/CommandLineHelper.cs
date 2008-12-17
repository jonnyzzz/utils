using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace exec64
{
  public class CommandLineHelper
  {
    private static readonly Regex CAN_NOT_QUOTE = new Regex(@"^[a-z\\/:0-9\._+=]*$", RegexOptions.IgnoreCase);
    private static readonly Regex SHOULD_QUOTE = new Regex("[|><\\s,;\"]+", RegexOptions.None);

    private static void SafeAppendParameter(StringBuilder sb, string text)
    {
      bool quote = SHOULD_QUOTE.IsMatch(text) || !CAN_NOT_QUOTE.IsMatch(text);
      if (quote)
        sb.Append('"');

      if (text.IndexOf('"') >= 0)
        text = text.Replace("\\\"", "\\\\\"").Replace("\"", "\\\"");

      sb.Append(text);

      if (quote)
      {
        if (text.EndsWith(@"\"))
          sb.Append('\\');

        sb.Append('"');
      }
    }

    public static string[] SplitCommandLine(string s)
    {
      const char separator = ' ';
      var result = new ArrayList();
      var builder = new StringBuilder();
      bool inQuotes = false;
      for (int i = 0; i < s.Length; i++)
      {
        char c = s[i];
        if (c == separator && !inQuotes)
        {
          if (builder.Length > 0)
          {
            result.Add(builder.ToString());
            builder = new StringBuilder();
          }
          continue;
        }

        if (c == '"' && !(i > 0 && s[i - 1] == '\\'))
        {
          inQuotes = !inQuotes;
        }
        builder.Append(c);
      }

      if (builder.Length > 0)
      {
        result.Add(builder.ToString());
      }
      return (string[])result.ToArray(typeof(string));
    }

    public static string Join(params string[] arguments)
    {
      return Join(0, arguments);
    }

    public static string Join(int offset, params string[] arguments)
    {
      var sb = new StringBuilder();
      bool isFirst = true;
      for (int i = offset; i < arguments.Length; i++)
      {
        string arg = arguments[i];
        if (!isFirst)
          sb.Append(' ');
        else
          isFirst = false;

        SafeAppendParameter(sb, arg);
      }
      return sb.ToString();
    }
  }
}