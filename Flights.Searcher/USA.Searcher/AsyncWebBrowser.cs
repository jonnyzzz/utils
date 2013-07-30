using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace USA.Searcher
{
  public static class AsyncWebBrowser
  {
    private static Action a(Action a)
    {
      return a;
    }

    public static Task NavigateAsync(this WebBrowser ie, string url)
    {
      return Task.Factory.StartNew(() =>
      {
        var m_MRE = new ManualResetEvent(false);
        WebBrowserDocumentCompletedEventHandler mWebBrowserOnDocumentCompleted = (_, e) => m_MRE.Set();
        ie.BeginInvoke(a(() =>
        {
          ie.Stop();
          ie.DocumentCompleted += mWebBrowserOnDocumentCompleted;
          ie.Navigate(new Uri(url));
        }));

        m_MRE.WaitOne();

        ie.BeginInvoke(a(() =>
        {
          ie.DocumentCompleted -= mWebBrowserOnDocumentCompleted;
        }));

      });
    }

  }
}


