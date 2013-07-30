using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using USA.Searcher.Data;

namespace USA.Searcher
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var progressForm = new Form();
      progressForm.Load += async (sender, args) =>
      {
        var enumerable = Tasks().ToArray();
        int c = 0;
        foreach (var request in enumerable)
        {
          c++;
          progressForm.BeginInvoke((Action) (() =>
          {
            progressForm.Name = progressForm.Text = string.Format("{0} of {1}", c, enumerable.Length);
          }));

          await CrawlDate(progressForm, request);
        }
        progressForm.Close();
      };
      Application.Run(progressForm);
    }

    private static async Task<Report> CrawlDate(Form ui, Request request)
    {
      var completed = new ManualResetEvent(false);
      Report r = null;

      Action initAction = () =>
      {
        var crawler = new SkyCrawler();
        crawler.Load += async delegate
        {
          r = await crawler.Analyze(request);

          var dates = request.From.ToDateString() + "-" + request.To.ToDateString();
          string path = Path.Combine("e:\\Flights", request.Source + "-" + request.Dectination,
            dates + ".xml");
          Directory.CreateDirectory(Path.GetDirectoryName(path));

          r.ToXml(path);
          crawler.Close();
          completed.Set();

          if (r.Data.Length > 0)
          {
            Console.Out.WriteLine("{0} -> {1}: {2}", dates, request.Dectination, r.Data[0].Price);
          }
        };
        crawler.Show();
      };

      await Task.Factory.FromAsync(
        ui.BeginInvoke(initAction),
        x => { ui.EndInvoke(x);  });

      return await Task.Factory.StartNew(() =>
      {
        completed.WaitOne();
        return r;
      });      
    }

    private static IEnumerable<Request> Tasks()
    {
      return
        from fromDate in Days(new DateTime(2013, 08, 20), 8, 8).Distinct()
        from toDate in Days(new DateTime(2013, 09, 11), 8, 8).Distinct()
        from dest in new[] { "chia" }
        where (toDate - fromDate).TotalDays >= 21 
        where (toDate - fromDate).TotalDays <= 26 
//        from dest in EAST_COAST.Values.OrderBy(x=>x)
        select new Request
        {
          From = fromDate,
          To = toDate,
          Source = "MUC",
          Dectination = dest
        };

    }

    private static readonly Dictionary<string, String> EAST_COAST = new Dictionary<string, string>
    {
      {"Washington-Dulles International Airport", "IAD"},
      {"Washington-Ronald Reagan National Airport", "DCA"},
      {"Atlanta-Hartsfield Jackson International Airport", "ATL"},
      {"Baltimore-Washington International Airport", "BWI"},
      {"Boston-Logan International Airport", "BOS"},
      {"Charlotte Douglas International Airport", "CLT"},
      {"Columbia Metropolitan Airport", "CAE"},
      {"Jacksonville International Airport", "JAX"},
      {"Miami International Airport", "MIA"},
      {"New York-John F Kennedy International Airport", "JFK"},
      {"New York- La Guardia Airport", "LGA"},
      {"Newark Liberty International Airport", "EWR"},
      {"Philadelphia International Airport", "PHL"},
      {"Pittsburgh International Airport", "PIT"},
      {"Raleigh-Durham International Airport", "RDU"},
      {"Richmond International Airport", "RIC"},
      {"Tampa International Airport", "TPA"}
    };

    private static IEnumerable<DateTime> Days(DateTime root, int from, int to)
    {
      yield return root;
      int c = 1;
      while (from > 0 || to > 0)
      {
        if (from > 0)
        {
          yield return root + TimeSpan.FromDays(-c);
          from--;
        }
        if (to > 0)
        {
          yield return root + TimeSpan.FromDays(+c);
          to--;
        }

        c++;
      }
    }
  }
}
