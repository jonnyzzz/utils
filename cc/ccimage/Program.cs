using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace cc
{
  class Program
  {
    private readonly Args args;

    [STAThread]
    public static int Main(string[] args)
    {
      return new Program(new Args(args)).Main();
    }

    public Program(Args args)
    {
      this.args = args;
    }

    private int Main()
    {
      string text = Console.In.ReadToEnd();
      Console.Out.WriteLine(text);
      Clipboard.SetText(text);    
      return 0;
    }
  }

  public class Args
  {
    private readonly List<String> myArgs;

    public Args(IEnumerable<string> args)
    {
      myArgs = new List<string>(args);
    }

    public string this[int i]
    {
      get
      {
        try
        {
          return myArgs[i];
        }
        catch
        {
          return null;
        }
      }
    }

    public bool Contains(string key)
    {
      return myArgs.Any(arg => arg.Equals("-" + key, StringComparison.InvariantCultureIgnoreCase));
    }

    public string Get(string key, string def)
    {
      string lookup = "-" + key + "=";
      foreach (var arg in myArgs)
      {        
        if (arg.StartsWith(lookup, StringComparison.InvariantCultureIgnoreCase))
        {
          return arg.Substring(lookup.Length);
        }
      }
      return def;
    }
  }
}
