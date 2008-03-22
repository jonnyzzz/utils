using System;
using System.ServiceProcess;
using System.Threading;

namespace BackUpService
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static int Main(string[] args)
    { 
      if (args.Length > 0 && args[0] == "console")
      {
        try
        {
          BackUpHolder holder = new BackUpHolder();
          holder.Start();
          Console.Out.WriteLine("Started.");
          while (Console.In.Read() == -1)
          {
            Thread.Sleep(100);
          }
          holder.Stop();
        } catch(Exception e)
        {
          Console.Error.WriteLine("e = {0}", e);
          return -1;
        }
      }
      else
      {
        ServiceBase.Run(new BackUpService());
      }
      return 0;       
    }
  }
}