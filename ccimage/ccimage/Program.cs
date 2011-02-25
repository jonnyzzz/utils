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
    private class Args
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
          } catch {
            return null;
          }
        }
      }

      public bool Contains(string key)
      {
        return myArgs.Any(arg => arg.Equals(key, StringComparison.InvariantCultureIgnoreCase));
      }
    }

    [STAThread]
    public static int Main(string[] args)
    {
      return Main(new Args(args));
    }

    private static int Main(Args args)
    {
      Console.Out.WriteLine("Utility to save images from Clipboard.");
      Console.Out.WriteLine("(C) 2011 Eugene Petrenko.");

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

    private static string GetFile(string ext)
    {
      string dir = Environment.CurrentDirectory;
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
}
