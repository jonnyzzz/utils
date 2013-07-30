using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using USA.Searcher;
using USA.Searcher.Data;

namespace USA.Anazyzer
{
  class Program
  {
    static void Main(string[] args)
    {
      string home = @"E:\Flights\MUC-nyca";
      var allReports = ListFiles(home)
        .AsParallel()
        .Where(File.Exists)
        .Where(x => x.EndsWith(".xml"))
        .Select(x=>x.FromXML<Report>())
        .Where(x=>x.Data.Any())
        .ToArray()
        .AsParallel();
      allReports.ForAll(x=>x.Data = x.Data.OrderBy(i=>i.Price).ToArray());
      allReports = allReports.OrderBy(x => x.Data.First().Price).ToArray().AsParallel();


      var slice = allReports.Take(12).OrderBy(report => Math.Abs(22 - (report.Request.To - report.Request.From).TotalDays));
      foreach (var report in slice)
      {
        Console.Out.WriteLine("{0} -> {1}", report.Request.Source, report.Request.Dectination);        
        Console.Out.WriteLine("Days: {0}", (report.Request.To - report.Request.From).TotalDays);
        Console.Out.WriteLine("From: {0}", report.Request.From.ToReadableDateString());
        Console.Out.WriteLine("To: {0}", report.Request.To.ToReadableDateString());

        var flight = report.Data.First();
        Console.Out.WriteLine("  Price: {0}", flight.Price);
        Console.Out.WriteLine("  Flight: {0}", flight.Company);
        Console.Out.WriteLine("  Stops: {0}; {1}", flight.To.StopPlaces.Join(), flight.Return.StopPlaces.Join());
        Console.Out.WriteLine();
      }

      Console.Out.WriteLine("Loaded reports: {0}", allReports.Count());
    }

    

    private static IEnumerable<string> ListFiles(string home)
    {
      var files = Directory.GetFiles(home);
      var rec = Directory.GetDirectories(home).AsParallel().SelectMany(ListFiles);
      return files.Union(rec);
    }
  }
}
