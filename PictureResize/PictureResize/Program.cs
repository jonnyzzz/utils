using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PictureResize
{
  class Program
  {
    static int Main(string[] args)
    {
      try
      {
        Console.Out.WriteLine("<Convert resolution>.exe <from folder> <to folder> <max resolution>");
        if (args.Length != 3) return -1;

        var fromPath = args.ElementAtOrDefault(0);
        var toPath = args.ElementAtOrDefault(1);
        var res = int.Parse(args.ElementAtOrDefault(2));
        if (!Directory.Exists(toPath))
          Directory.CreateDirectory(toPath);

        foreach (var jpg in Directory.EnumerateFiles(fromPath, "*.jpg"))
        {
          Console.Out.WriteLine("Converting {0}", jpg);

          using (var imp = Image.FromFile(jpg))
          {
            int resX;
            int resY;

            if (imp.Width > imp.Height)
            {
              resX = res;
              resY = (int) (1 + (imp.Height / (double) imp.Width * res));
            }
            else
            {
              resY = res;
              resX = (int) (1 + (imp.Width / (double) imp.Height * res));
            }

            var bm = new Bitmap(resX, resY);
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
      catch(Exception e)
      {
        Console.Out.WriteLine("Unknown error. " + e);
        return -1;
      }
    }
  }
}
