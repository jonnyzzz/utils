using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ccimage
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
      Console.Out.WriteLine("Utility to save images from Clipboard.");
      Console.Out.WriteLine("(C) 2011 Eugene Petrenko.");

      Console.Out.WriteLine("Usage: ");
      Console.Out.WriteLine(" ccimage.exe [-show] [-dir=<dir>]");
      Console.Out.WriteLine("         -show        show captured image");
      Console.Out.WriteLine("         -dir=<dir>   saves image to specified dir instead of working dir");
      Console.Out.WriteLine("");
      Console.Out.WriteLine("  captured image path will be put into clipboard.");

      Image image = Clipboard.GetImage();
      if (image == null)
      {
        Console.Out.WriteLine("No Image was detected in clipboard.");
        return 1;
      }


      if (args.Contains("show"))
      {
        var f = new Form();
        var c = new PictureBox
                  {
                    Dock = DockStyle.Fill,
                    Image = image,
                    SizeMode = PictureBoxSizeMode.AutoSize
                  };
        f.Controls.Add(c);
        f.ShowDialog();
      }

      var file = GetFile(".png");
      image.Save(file, ImageFormat.Png);

      Console.Out.WriteLine("Image saved to: " + file);

      Clipboard.SetText(file);
      return 0;
    }

    private string GetFile(string ext)
    {
      string dir = args.Get("dir", Environment.CurrentDirectory);
      int cnt = 0;
      while (true)
      {
        string file = Path.Combine(dir, "Image_" + cnt.ToString("0000") + ext);
        if (!File.Exists(file)) return file;
        cnt++;
        cnt++;
      }      
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
