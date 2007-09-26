using System;

namespace javaTime
{
  public static class TimeUtil
  {
    private static readonly DateTime JAVA_BASE_TIME = new DateTime(1970, 1, 1);

    public static long ToJavaTime(DateTime time)
    {
      return ((long)(time.ToUniversalTime() - JAVA_BASE_TIME).TotalMilliseconds);
    }

    public static DateTime FromJavaTime(long javaTime)
    {
      TimeSpan span = new TimeSpan(javaTime * TimeSpan.TicksPerMillisecond);
      return (JAVA_BASE_TIME + span).ToLocalTime();
    }
  }
}