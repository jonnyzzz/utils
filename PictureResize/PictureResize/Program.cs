using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace PictureResize
{
  class Program
  {
    static int Main(string[] args)
    {
      try
      {
        Console.Out.WriteLine("<Convert resolution> <from folder> <to folder> <resolutionX> <resolutionY>");
        if (args.Length != 4) return -1;

        var fromPath = args.ElementAtOrDefault(0);
        var toPath = args.ElementAtOrDefault(1);
        var resolutionX = int.Parse(args.ElementAtOrDefault(2));
        var resolutionY = int.Parse(args.ElementAtOrDefault(3));

        foreach (var jpg in Directory.EnumerateFiles(fromPath, "*.jpg"))
        {
          Console.Out.WriteLine("Converting {0}", jpg);

          using (var imp = Image.FromFile(jpg))
          {
            var bm = new Bitmap(resolutionX, resolutionY);
            using (var g = Graphics.FromImage(bm))
            {
              g.DrawImage(imp, 0, 0, bm.Width, bm.Height);
            }
            var path = Path.Combine(toPath, Path.GetFileName(jpg));
            bm.Save(path, ImageFormat.Jpeg);
          }
        }
        return 0;
      }
      catch
      {
        Console.Out.WriteLine("Unknown error. ");
        return -1;
      }
    }
  }
}
