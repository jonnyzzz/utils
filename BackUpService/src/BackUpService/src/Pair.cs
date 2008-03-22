using System;

namespace BackUpService
{
  public class Pair<TA, TB> : IEquatable<Pair<TA, TB>>
  {
    public readonly TA A;
    public readonly TB B;

    public Pair(TA a, TB b)
    {
      A = a;
      B = b;
    }


    public bool Equals(Pair<TA, TB> pair)
    {
      if (pair == null) return false;
      return Equals(A, pair.A) && Equals(B, pair.B);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(this, obj)) return true;
      return Equals(obj as Pair<TA, TB>);
    }

    public override int GetHashCode()
    {
      return A.GetHashCode();
    }
  }

  public static class Pair
  {
    public static Pair<A,B> Create<A,B>(A a, B b)
    {
      return new Pair<A, B>(a, b);
    }
  }
}