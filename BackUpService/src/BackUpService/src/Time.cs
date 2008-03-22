using System;
using System.Xml.Serialization;

namespace BackUpService
{
  [Serializable][XmlRoot("time")]
  public class Time
  {
    public int Hour;
    public int Minute;

    public Time()
    {
    }

    public Time(int hour, int minute)
    {
      Hour = hour;
      Minute = minute;
    }
  }
}