using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KennenzeichenMunchen
{
  public class Numb : IComparable<Numb>
  {
    private static Regex REG = new Regex(@"M\s*(\w{2})\s*(\d{4})", RegexOptions.Compiled);
    public string XX { get; set; }
    public string Num { get; set; }
    public int Rank { get; set; }

    public Numb(string s)
    {
      var m = REG.Match(s.Trim());
      XX = m.Groups[1].Value;
      Num = m.Groups[2].Value;
      Rank = MakeRank();
    }

    public int CompareTo(Numb other)
    {
      int x;
      if ((x = (Rank.CompareTo(other.Rank))) != 0) return -x;
      if ((x = (XX.CompareTo(other.XX))) != 0) return x;
      if ((x = (Num.CompareTo(other.Num))) != 0) return x;
      return 0;
    }

    public override string ToString()
    {
      string ranq = "" + Rank;
      while (ranq.Length <= 6) ranq = " " + ranq;
      return string.Format("[{0}]  {1}", ranq, ToShortString());
    }

    public string ToShortString()
    {
      return "M " + XX + "  " + Num;
    }

    private static string _96(string _)
    {
      return _.Replace("9", "?").Replace("6", "?");
    }

    private static string _17(string _)
    {
      return _.Replace("1", "?").Replace("7", "?");
    }

    private static string _38(string _)
    {
      return _.Replace("3", "?").Replace("8", "?");
    }

    private static bool poly(string _)
    {
      return _ == new string(_.ToCharArray().Reverse().ToArray());
    }

    public int MakeRank()
    {
      double rank = 1;

      if (poly(Num)) rank += 150;
      if (poly(_96(Num))) rank += 140;
      if (poly(_17(Num))) rank += 110;
      if (poly(_38(Num))) rank += 110;

      rank += Math.Pow(4.0 / Num.ToCharArray().Distinct().Count(), 3) / 2;
      
      if (Num[0] == Num[1] && Num[2] == Num[3]) rank += 120;  //xxyy
      if (Num[0] == Num[3] && Num[1] == Num[2]) rank += 120;  //xyxy
      
      if (Num[0] == Num[3]) rank += 25;  //x??x
      if (Num[2] == Num[1]) rank += 25;  //?xx?
      if (Num[0] == Num[2]) rank += 25;  //x?x?

      if (Num[0] == Num[1] && Num[1] == Num[2]) rank += 65;  //xxx?
      if (Num[1] == Num[2] && Num[2] == Num[3]) rank += 65;  //?xxx

//      rank += 25 * Math.Pow(Num.ToCharArray().Count(x => x == '8'), 1);
//      rank += 25 * Math.Pow(Num.ToCharArray().Count(x => x == '7'), 1);
//      rank += 25 * Math.Pow(Num.ToCharArray().Count(x => x == '0'), 1);

      var sims = "XUAIWTYHVM".ToCharArray().Select(x => "" + x).ToArray();
      rank += sims.Where(x => XX == x + "M").Sum(x => 100);
      
      if (poly("M" + XX)) rank += 100;
      if (XX.EndsWith("M")) rank += 20;
      if (XX.StartsWith("M")) rank += 20;

      return (int) Math.Ceiling(rank);
    }

    public override bool Equals(object obj)
    {
      return NumbEqualityComparer.THIS.Equals(this, obj as Numb);
    }

    public override int GetHashCode()
    {
      return NumbEqualityComparer.THIS.GetHashCode(this);
    }
  }

  public class NumbEqualityComparer : IEqualityComparer<Numb>
  {
    public static readonly IEqualityComparer<Numb> THIS = new NumbEqualityComparer();

    public bool Equals(Numb x, Numb y)
    {
      if (ReferenceEquals(x, y)) return true;
      if (x == null || y == null) return false;
      return x.XX == y.XX && y.Num == x.Num;
    }

    public int GetHashCode(Numb o)
    {
      unchecked
      {
        return o.Rank << 23 + 31*(o.XX.GetHashCode() + 31*o.Num.GetHashCode());
      }
    }
  }
}

