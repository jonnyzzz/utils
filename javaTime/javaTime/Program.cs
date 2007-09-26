using System;
using System.Windows.Forms;

namespace javaTime
{  
  class Program
  {
    [STAThread]
    static void Main(params string[] args)
    {
      if (args.Length == 0)
      {
        long time = TimeUtil.ToJavaTime(DateTime.Now);
        Console.Out.WriteLine("time = {0}", time);

        Clipboard.SetText(time.ToString());
      } else if (args.Length == 1) {
        string code;
        if (args[0] != "c")
        {
          code = args[0];
        } else
        {
          code = Clipboard.GetText();
        }

        long v;
        if (long.TryParse(code, out v))
        {
          DateTime time = TimeUtil.FromJavaTime(v);
          Console.Out.WriteLine("time = {0}", time);
        }
      }
    }
  }
}
