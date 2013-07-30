using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using mshtml;
using USA.Searcher.Data;

namespace USA.Searcher
{
  public class SkyScanner
  {
    private readonly WebBrowser myIE;

    public SkyScanner(WebBrowser ie)
    {
      myIE = ie;
    }

    private static Action a(Action a)
    {
      return a;
    }

    public async Task<Report> ScanAsync(Request request)
    {
      var r = new Report
      {
        Request = request,
        URL = string.Format(
          "http://www.skyscanner.de/flights/{0}/{1}/{2}/{3}",
          request.Source.ToLower(),
          request.Dectination.ToLower(),
          request.From.ToString("yyMMdd"),
          request.To.ToString("yyMMdd"))
      };

      await ScanAsync(r, r.AddFlight);
      return r;
    }

    private async Task ScanAsync(Report r, Action<ReturnFlight> addFlight)
    {
      await myIE.NavigateAsync(r.URL);
      if (!await WaitForLoadAsync()) return;
      await ProcessResultsAync(addFlight);
      int page = 2;
      while (page <= 8)
      {
        if (!await NextPage(page)) break;
        Thread.Sleep(500);
        if (!await WaitForLoadAsync()) return;
        await ProcessResultsAync(addFlight);
        page++;
      }
    }

    private Task<bool> NextPage(int id)
    {
      return Task.Factory.StartNew(() =>
      {
        bool found = false;
        myIE.Invoke(a(() =>
        {
          dynamic root = myIE.Document.DomDocument;
          dynamic container = root.getElementById("day_paging");
          foreach (IHTMLElement page in (getElementsByClassNameEx(container, "pages", "page", "p")))
          {
            if (InnerTexts(page) == "" + id)
            {
              page.click();
              found = true;
              return;
            }
          }
        }));
        return found;
      });
    }

    private Task<bool> WaitForLoadAsync()
    {
      return Task.Factory.StartNew(() =>
      {
        bool detected = false;
        bool failed = false;
        while (!detected)
        {
          Thread.Sleep(300);
          myIE.Invoke(a(() =>
          {
            dynamic root = myIE.Document.DomDocument;
            failed |= IsNotEmpty(getElementsByClassNameEx(root, "viewport", "day_resultslist", "noresults", "cushion"))
              && ((string)InnerTexts(getElementsByClassNameEx(root, "message"))).Trim().Length == 0;
            detected |= failed;
            detected |= IsNotEmpty(root.getElementsByClassName("message"))
                        && IsNotEmpty(root.getElementsByClassName("message-complete"));
          }));
        }
        return !failed;
      });
    }

    private Task ProcessResultsAync(Action<ReturnFlight> addFlight)
    {
      return Task.Factory.StartNew(() => myIE.Invoke(a(() => ProcessResultsImpl(addFlight))));
    }

    private void ProcessResultsImpl(Action<ReturnFlight> addFlight)
    {
      dynamic doc = myIE.Document.DomDocument;
      dynamic rows = getElementsByClassNameEx(doc, "viewport", "day_resultslist", "row");
      foreach (var row in rows)
      {
        var rf = new ReturnFlight();
        rf.To = ParseFlight(getFirstElementByClassNameEx(row, "flight-o"));
        rf.Return = ParseFlight(getFirstElementByClassNameEx(row, "flight-i"));
        rf.Price = ParsePrice(getFirstElementByClassNameEx(row, "mainquote", "px"));
        rf.Company = InnerTexts(getElementsByClassNameEx(row, "carr"));
        
        addFlight(rf);
      }    
    }

    private static Flight ParseFlight(dynamic row)
    {
      var flight = new Flight();
      flight.FlightHours = InnerTexts(getElementsByClassNameEx(row, "path", "dur"));
      flight.StopPlaces = Enumerable.ToArray(InnerText(getElementsByClassNameEx(row, "path", "sta")));
      flight.DepartureTime = InnerTexts(getElementsByClassNameEx(row, "ileg", "tim-dep"));
      flight.ArriveTime = InnerTexts(getElementsByClassNameEx(row, "ileg", "tim-arr"));
      return flight;
    }

    private static dynamic getFirstElementByClassNameEx(dynamic h, params string[] clazzez)
    {
      foreach (var el in getElementsByClassNameEx(h, clazzez))
      {
        return el;
      }
      return null;
    }

    private static IEnumerable<dynamic> getElementsByClassNameEx(dynamic h, params string[] clazzez)
    {
      if (clazzez.Length == 0)
      {
        yield return h;
        yield break;
      }

      string clazz = clazzez[0];
      string[] req = clazzez.Skip(1).ToArray();

      foreach (var el in h.getElementsByClassName(clazz))
      {
        foreach (var e in getElementsByClassNameEx(el, req))
        {
          yield return e;
        }
      }
    }

    private static int ParsePrice(dynamic d)
    {
      var s = ((object) d.InnerText).ToString();
      return int.Parse(s.Replace("&nbsp;", "").Replace("€", "").Replace(" ", "").Replace(".", ""));
    }

    private static string InnerTexts(dynamic d)
    {
      return string.Join("", InnerText(d));
    }

    private static IEnumerable<string> InnerText(dynamic d)
    {
      if (d is IEnumerable)
      {
        foreach (IHTMLElement element in d)
        {
          yield return SingleInnerText(element);
        }
      }
      else
      {
        yield return SingleInnerText(d);
      }
    }

    private static string SingleInnerText(IHTMLElement element)
    {
      if (String.Equals(element.tagName, "img", StringComparison.CurrentCultureIgnoreCase))
      {
        return element.getAttribute("alt") ?? "";
      }
      return (element.innerText ?? "").Trim();
    }

    private static bool IsNotEmpty(dynamic d)
    {
      try
      {
        foreach (var _ in d) return true;
      }
      catch
      {
        //NOP
      }
      return false;
    }
  }
}