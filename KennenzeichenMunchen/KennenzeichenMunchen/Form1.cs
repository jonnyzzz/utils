using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace KennenzeichenMunchen
{
  public partial class Form1 : Form
  {
    private readonly HashSet<Numb> myNumbers = new HashSet<Numb>(); 

    public Form1(IEnumerable<Numb> allNumbers)
    {
      InitializeComponent();
      webBrowser1.ScriptErrorsSuppressed = true;

      foreach (var numb in allNumbers)
      {
        myNumbers.Add(numb);
      }
      UpdateNumbers();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      webBrowser1.Navigate("https://www10.muenchen.de/WuKe/");
      webBrowser1.Navigating += (sender, args) =>
      {
        args.Cancel = !args.TargetFrameName.EndsWith("");
      };
      webBrowser1.DocumentCompleted += (sender, args) =>
      {
        if (!ParseNumbers(webBrowser1.DocumentText)) return;

        ThreadPool.QueueUserWorkItem(delegate
        {
          Thread.Sleep(1000);
          BeginInvoke((Action) (delegate
          {
            HtmlElement htmlElement =
              webBrowser1.Document.GetElementsByTagName("input")
                .Cast<HtmlElement>().FirstOrDefault(x => x.GetAttribute("name") == "pbWeiter");

            if (htmlElement != null)
            {
              ((dynamic) htmlElement.DomElement).click();
            }
          }));
        });
      };
    }

    private bool ParseNumbers(string text)
    {
      IEnumerable<Numb> matchText = MatchText(text);
      bool news = matchText.Aggregate(false, (current, e) => current | myNumbers.Add(e));
      if (!news) return false;

      UpdateNumbers();
      return true;
    }

    private void UpdateNumbers()
    {
      var data = new List<Numb>(myNumbers);
      data.Sort();

      string @join = string.Join("\r\n", data);
      NumsStore.AddMoreNumbers(myNumbers);

      textBox1.Text = @join;
    }

    public static IEnumerable<Numb> MatchText(string text)
    {
      var ns = new List<Numb>();
      for (Match match = Regex.Match(text, @">\s*(M\s*.{2}\d{3,4})\s*</"); match.Success; match = match.NextMatch())
      {
        var result = new Numb(match.Groups[1].Value);
        Console.Out.WriteLine("-> {0}", result);
        ns.Add(result);
      }
      return ns;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      myNumbers.Clear();
      UpdateNumbers();
    }
  }
}
