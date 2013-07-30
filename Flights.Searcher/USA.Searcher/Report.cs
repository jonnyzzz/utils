using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace USA.Searcher.Data
{
  [Serializable]
  [XmlRoot("request")]
  public class Request
  {
    [XmlElement("from")]
    public DateTime From { get; set; }

    [XmlElement("to")]
    public DateTime To { get; set; }

    [XmlElement("source")]
    public string Source { get; set; }

    [XmlElement("destination")]
    public string Dectination { get; set; }
  }

  [Serializable]
  [XmlRoot("report")]
  public class Report
  {
    private ReturnFlight[] myData;

    [XmlElement("request")]
    public Request Request { get; set; }
    
    [XmlAttribute("url")]
    public string URL { get; set; }

    [XmlArray("flights"), XmlArrayItem("flight")]
    public ReturnFlight[] Data
    {
      get { return myData ?? new ReturnFlight[0]; }
      set { myData = value ?? new ReturnFlight[0]; }
    }
  }

  public static class DataExtensions
  {
    public static void AddFlight(this Report r, ReturnFlight d)
    {
      r.Data = (r.Data ?? new ReturnFlight[0]).Union(new[] {d}).ToArray();
    }

    public static string ToDateString(this DateTime r)
    {
      return r.ToString("yyMMdd");
    }

    public static string ToReadableDateString(this DateTime r)
    {
      return r.ToString("MMMM dd");
    }

    public static string Join(this IEnumerable<string> r, string sep = ", ")
    {
      return string.Join(sep, r);
    }
  }

  [Serializable]
  [XmlRoot("return-flight")]
  public class ReturnFlight
  {
    [XmlAttribute("company")]
    public string Company { get; set; }

    [XmlElement("to")]
    public Flight To { get; set; }

    [XmlElement("return")]
    public Flight Return { get; set; }

    public int? Price { get; set; }
  }

  [Serializable]
  [XmlRoot("flight")]
  public class Flight
  {
    private string[] myStopPlaces;

    [XmlAttribute("stops")]
    public int Stops
    {
      get { return StopPlaces == null ? 0 : StopPlaces.Length; }
      set { /**  NOOP */ }
    }

    [XmlArray("stops"), XmlArrayItem("stop")]
    public string[] StopPlaces
    {
      get { return myStopPlaces ?? new string[0]; }
      set { myStopPlaces = value ?? new string[0]; }
    }

    [XmlAttribute("dep")]
    public string DepartureTime { get; set; }

    [XmlAttribute("arr")]
    public string ArriveTime { get; set; }

    [XmlAttribute("time")]
    public string FlightHours { get; set; }
  }
}
