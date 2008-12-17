using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace exec64
{
  class Program
  {
    static int Main(string[] args)
    {
      if (args.Length == 0 || args[0] == "/?" || args[0] == "--help")
      {
        Console.Out.WriteLine("Run process in x64 shell.");
        Console.Out.WriteLine("Usage:");
        Console.Out.WriteLine(" <processname.exe> <program to run> [args]");
        return 0;
      }
      try
      {
        var pi = new ProcessStartInfo
                   {
                     UseShellExecute = true,
                     Arguments = CommandLineHelper.Join(1, args),
                     FileName = args[0],
//                     RedirectStandardError = true,
 //                    RedirectStandardInput = true,
//                     RedirectStandardOutput = true,
                     WorkingDirectory = Environment.CurrentDirectory
                   };

        Process start = Process.Start(pi);
        if (start == null)
        {
          Console.Error.WriteLine("Failed to start process. ");
          return -1;
        }

//        var output = CopyThread(start.StandardOutput, Console.Out);
//        var error = CopyThread(start.StandardError, Console.Error);
//        var input = CopyThread(Console.In, start.StandardInput);

        start.WaitForExit();

//        input.Interrupt();
//        error.Join();
//        output.Join();
        return start.ExitCode;
      } catch (Exception e)
      {
        Console.Error.WriteLine("Error: " + e);
        return -2;
      }
    }

    public delegate void Dispose();

    private static Thread CopyThread(TextReader rdr, TextWriter wr)
    {      
      var th = new Thread(delegate()
                               {
                                 for (int i; ((i = rdr.Read()) >= 0); wr.Write((char)i)) ;
                               });
      th.Start();
      return th;      
    }

  }
}
