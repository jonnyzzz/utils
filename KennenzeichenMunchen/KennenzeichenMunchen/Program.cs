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
      IEnumerable<Numb> allNumbers = NumsStore.LoadNumbers();

      


      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1(allNumbers));


    }
  }
}
