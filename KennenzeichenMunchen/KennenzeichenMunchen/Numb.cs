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
      return string.Format("[{0:######}]  M {1} {2}", Rank, XX, Num);
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
      int rank = 1;

      if (poly(Num)) rank += 50;
      if (poly(_96(Num))) rank += 40;
      if (poly(_17(Num))) rank += 10;
      if (poly(_38(Num))) rank += 10;

      rank += 1000/
              (1 +
               (1 + Math.Abs(Num[0] - Num[1]))*(1 + Math.Abs(Num[1] - Num[2]))*(1 + Math.Abs(Num[2] - Num[3]))*
               (1 + Math.Abs(Num[3] - Num[0])));
      
      if (Num[0] == Num[1] && Num[2] == Num[3]) rank += 40;
      if (Num[0] == Num[3] && Num[1] == Num[2]) rank += 40;
      
      if (Num[0] == Num[3]) rank += 5;
      if (Num[2] == Num[1]) rank += 5;
      if (Num[0] == Num[2]) rank += 5;

      if (Num[0] == Num[1] && Num[1] == Num[2]) rank += 45;
      if (Num[1] == Num[2] && Num[2] == Num[3]) rank += 45;


      rank += 20 * Num.ToCharArray().Count(x => x == '4');
      rank += 20 * Num.ToCharArray().Count(x => x == '5');
      rank += 20 * Num.ToCharArray().Count(x => x == '8');
      rank += 20 * Num.ToCharArray().Count(x => x == '9');
      rank += 20 * Num.ToCharArray().Count(x => x == '6');
      rank += 25 * Num.ToCharArray().Count(x => x == '7');

      var difs = Num.ToCharArray().Distinct().Count();
      rank += (4*4*4/difs*difs*difs)/3;

      if (poly("M" + XX)) rank += 400;
      if (XX == "XM") rank += 301;
      if (XX == "UM") rank += 301;
      if (XX == "AM") rank += 301;
      if (XX == "IM") rank += 301;
      if (XX == "WM") rank += 301;
      if (XX == "TM") rank += 301;
      if (XX == "YM") rank += 301;
      if (XX == "HM") rank += 301;
      if (XX == "VM") rank += 301;
      if (XX == "NM") rank += 301;
      if (XX.EndsWith("M")) rank += 233;

      return rank;
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