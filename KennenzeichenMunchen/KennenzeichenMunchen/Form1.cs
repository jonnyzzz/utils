using System;
using System.Collections.Generic;
using System.Drawing;
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
      myWeb.ScriptErrorsSuppressed = true;

      textBox2.Font = FESchrift.Font(18);
      textBox1.Font = new Font("COURIER NEW", 10);

      textBox1.SelectionChanged += (sender, args) =>
      {
        try
        {
          var text = "\n" + textBox1.Text;
          int pos = 1 + textBox1.SelectionStart;
          if (!(pos >= 0 && pos < text.Length))
          {
            textBox2.Text = "";
          }
          else
          {
            int start = pos;
            int end = pos;
            while (start >= 0 && text[start] != '\n') start--;
            while (end < text.Length && text[end] != '\n') end++;

            var line = text.Substring(start, end - start);
            textBox2.Text = new Numb(line).ToShortString();
          } 
        }
        catch
        {
          //NOP
        }
      };

      textBox1.TextChanged += (sender, args) => textBox2.Text = "";

      foreach (var numb in allNumbers)
      {
        myNumbers.Add(numb);
      }
      UpdateNumbers();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      myWeb.Navigate("https://www10.muenchen.de/WuKe/");
      myWeb.Navigating += (sender, args) =>
      {
        args.Cancel = !args.TargetFrameName.EndsWith("");
      };
      myWeb.DocumentCompleted += (sender, args) =>
      {
        if (!ParseNumbers(myWeb.DocumentText)) return;

        ThreadPool.QueueUserWorkItem(delegate
        {
          Thread.Sleep(1000);
          BeginInvoke((Action) (delegate
          {
            HtmlElement htmlElement =
              myWeb.Document.GetElementsByTagName("input")
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
