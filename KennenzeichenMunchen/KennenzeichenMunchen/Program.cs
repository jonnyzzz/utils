using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KennenzeichenMunchen
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      IEnumerable<Numb> allNumbers = NumsStore.LoadNumbers();
      Application.Run(new Form1(allNumbers));
    }
  }
}
