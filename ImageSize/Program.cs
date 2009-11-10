using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageSize
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      if (args.Length != 1)
      {
        Console.Out.WriteLine("<assembly> imagePath");
        return;
      }

      var image = Image.FromFile(args[0]);

      Console.Out.WriteLine("Image: {0}", args[0]);
      Console.Out.WriteLine("Size: {0} {1}", image.Width, image.Height);

      Clipboard.SetText(string.Format("{0} {1}", image.Width, image.Height));
    }
  }
}
