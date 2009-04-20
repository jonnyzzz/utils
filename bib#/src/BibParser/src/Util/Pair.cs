using System;
using System.Collections.Generic;
using System.Text;

namespace EugenePetrenko.BibParser.Util
{
  public class Pair<A,B>
  {
    public readonly A First;
    public readonly B Second;

    public Pair(A first, B second)
    {
      First = first;
      Second = second;
    }
  }

  public static class Pair
  {
    public static Pair<A,B> Of<A,B>(A a, B b)
    {
      return new Pair<A, B>(a, b);
    }
  }

  public static class Util
  {
    public static string JoinString<T>(this IEnumerable<T> data, Func<T,string> toString, string sep)
    {
      bool isFirst = true;
      var sb= new StringBuilder();
      foreach (var d in data)
      {
        if (!isFirst)
        {
          sb.Append(sep);
        } else
        {
          isFirst = false;
        }
        sb.Append(toString(d));
      }
      return sb.ToString();
    }

    public static string JoinString<T>(this IEnumerable<T> data, string sep)
    {
      return data.JoinString(x => x.ToString(), sep);
    }
  }
}