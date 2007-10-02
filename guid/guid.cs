using System;
using System.Windows.Forms;

namespace javaTime
{  
  class Program
  {
    [STAThread]
    static void Main(params string[] args)
    {
      string guid = "{" + Guid.NewGuid() + "}";

        Console.Out.WriteLine("time = {0}", guid);

        Clipboard.SetText(guid);
    } 
  }
}
